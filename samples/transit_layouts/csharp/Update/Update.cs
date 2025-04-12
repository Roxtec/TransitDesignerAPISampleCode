using System;
using RtdApiCodeSamples;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

var options = Common.ParseCommandLine(args);
var transitId = options.TransitIds.Length > 0
    ? options.TransitIds[0]
    : Common.AskTransitId();

Console.WriteLine($"Project ID: {options.ProjectId}");
Console.WriteLine($"Project API key: {options.ProjectApiKey}");
Console.WriteLine($"Transit ID: {transitId}");

// Create a client and set to authenticate using the API key
var client = Common.CreateClient(options);

// Get the transit first, so that we can compare fill rate before and after the update.
// Also, while the update API supports patching (sending partial data), we must send
// all cables rather than only new ones.
var documentBefore = await client.GetTransitLayoutAsync(options.ProjectId, transitId);

// Take existing cables, then add 4 new ones
var cables = documentBefore.Data.Attributes.Cables;
var newCables = cables.Concat(GenerateAdditionalCables(cables, 4));

// Build a document with transit update data.
// Attributes only need to contain top-level properties to update, in this case Cables.
var createDocument = new SingleTransitLayoutCreateUpdateDocument
{
    Data = new()
    {
        Id = transitId.ToString(), // needed for update
        Type = Common.TransitLayoutsType,
        Attributes = new()
        {
            Cables = newCables.ToArray()
        },
        Relationships = new()
        {
            Project = Common.CreateProjectRelationship(options.ProjectId)
        }
    }
};

var resultDocument = await client.UpdateTransitLayoutAsync(options.ProjectId, transitId,
    createDocument);

Console.WriteLine("");
Console.WriteLine("The transit was successfully updated!");
Console.WriteLine($"Fill rate before : {FillRatePct(documentBefore)}");
Console.WriteLine($"Fill rate after  : {FillRatePct(resultDocument)}");

static string FillRatePct(SingleTransitLayoutDocument document)
{
    var rate = document.Data.Attributes.PackingResult.FillRate * 100;
    var rateStr = rate.ToString("F2", CultureInfo.InvariantCulture);
    return $"{rateStr} %";
}

static IEnumerable<Cable> GenerateAdditionalCables(ICollection<Cable> cables, int count)
{
    var existingIds = new HashSet<string>(cables.Select(cable => cable.Id));
    return Enumerable.Repeat(0, count).Select(_ =>
    {
        var id = GenerateId();
        return new Cable {Id = id, Diameter = 20};
    });

    string GenerateId()
    {
        var number = 0;
        string newId;
        do
        {
            number++;
            newId = $"a{number}";
        } while (existingIds.Contains(newId));

        existingIds.Add(newId);
        return newId;
    }
}
