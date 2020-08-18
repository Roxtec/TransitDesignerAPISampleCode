using RtdApiCodeSamples;
using System;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main(string[] args)
    {
        var options = Common.ParseCommandLine(args);
        
        Console.WriteLine($"Project ID: {options.ProjectId}");
        Console.WriteLine($"Project API key: {options.ProjectApiKey}");

        // Create a client and set to authenticate using the API key
        var client = Common.CreateClient(options);

        // Build a document with settings for the new transit
        // There are 2 errors here:
        // 1. We have a duplicate cable ID (id-a).
        // 2. The frame part number is not valid.
        var createDocument = new SingleTransitLayoutCreateUpdateDocument
        {
            Data = new TransitLayoutCreateUpdateResource
            {
                Type = Common.TransitLayoutsType,
                Attributes = new Attributes
                {
                    Name = "TestError",
                    Cables = new[]
                    {
                        new Cable {Id = "id-a", Diameter = 20},
                        new Cable {Id = "id-a", Diameter = 20},
                    },
                    Frame = new Frame
                    {
                        PartNumber = "unknown"
                    }
                },
                Relationships = new Relationships
                {
                    Project = Common.CreateProjectRelationship(options.ProjectId)
                }
            }
        };

        try
        {
            // Send the transit create request to the Transit Designer server.
            // We expect it to fail.
            var resultDocument = await client.CreateTransitLayoutAsync(options.ProjectId, createDocument, default);

            Console.WriteLine($"Unexpected! The create request did not fail (resulting transit ID is {resultDocument.Data.Id}).");
        }
        catch (ApiException<ErrorList> ex)
        {
            Console.WriteLine("Errors");
            Console.WriteLine("------");
            foreach (var error in ex.Result.Errors)
            {
                Console.WriteLine($"* {error.Title}: {error.Detail}");
            }
        }
    }
}
