using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using RtdApiCodeSamples;

var options = Common.ParseCommandLine(args);
var transitIds = options.TransitIds.Length > 0
    ? options.TransitIds
    : Common.AskTransitIds();

var transitIdStrings = transitIds.Select(id => id.ToString()).ToList();

// Create a client and set to authenticate using the API key
var client = Common.CreateClient(options);

// Build a document that contains options for creating transit documents.
var createJobDocument = new CreateTransitDocumentsJobDocument
{
    Data = new()
    {
        Type = Common.TransitDocumentsType,
        Attributes = new()
        {
            TransitIds = transitIdStrings,
            DocumentTypes = new()
            {
                BillOfMaterial = new() {Xlsx = true},
                Certificate = false, // only available if at least one transit has a certificate
                Drawing = new() {Dxf = true, Pdf = true},
                FrameModel = new() {Revit = true, Step = true},
                InstallationInstruction = true,
                SpecificationDocument = false // not available for all project categories
            }
        },
        Relationships = new()
        {
            Project = Common.CreateProjectRelationship(options.ProjectId)
        }
    }
};

try
{
    // Create the server job; the server responds with a job status document.
    var jobDocument = await client.CreateTransitDocumentsJobAsync(options.ProjectId, createJobDocument);
    var jobId = Guid.Parse(jobDocument.Data.Id);

    Console.WriteLine("");
    Console.WriteLine($"Documents job successfully created, got job ID {jobId}");

    var pctComplete = jobDocument.Data.Attributes.PercentageComplete;
    Console.WriteLine($"% complete : {pctComplete:F2}");

    // Query job status periodically, but not too often
    while (pctComplete < 100d)
    {
        // Wait 1.5 seconds before checking status again.
        await Task.Delay(1500);

        var statusDocument = await client.GetTransitDocumentsJobAsync(options.ProjectId, jobId);
        pctComplete = statusDocument.Data.Attributes.PercentageComplete;

        Console.WriteLine($"% complete : {pctComplete:F2}");
    }

    Console.WriteLine("");
    Console.WriteLine("Document generation complete, starting to download ...");

    // Download using HttpClient, see comment above about NSwag-generated DownloadTransitDocumentsAsync.
    var response = await client.DownloadTransitDocumentsAsync(options.ProjectId, jobId);

    // Extract the file name from the Content-Disposition header.
    // This is optional; the file can be saved under any name.
    var filename = GetFilenameFromResponse(response);

    await using var outputStream = File.OpenWrite(filename);
    await response.Stream.CopyToAsync(outputStream);

    Console.WriteLine("");
    Console.WriteLine($"Download complete; the documents were saved in {filename}");
}
catch (ApiException<ErrorList> ex)
{
    foreach (var err in ex.Result.Errors)
    {
        Console.WriteLine($"{err.Title}: {err.Detail}");
    }
}

static string GetFilenameFromResponse(FileResponse response)
{
    if (response.Headers.TryGetValue("Content-Disposition", out var values))
    {
        var headerValue = ContentDispositionHeaderValue.Parse(values.First());
        return headerValue.FileNameStar ?? headerValue.FileName;
    }
    return "downloaded";
}
