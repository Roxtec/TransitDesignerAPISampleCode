using RtdApiCodeSamples;
using System;
using System.Threading;
using Newtonsoft.Json;

const string S6X2Aisi316PartNumber = "S006000000221";
const string LeveloutPackAlgorithm = "levelout";
const string LeveloutByCategoryPackAlgorithm = "leveloutbycategory";
    
var options = Common.ParseCommandLine(args);

Console.WriteLine("Please enter a transit name:");
var transitName = Console.ReadLine();

Console.WriteLine($"Project ID: {options.ProjectId}");
Console.WriteLine($"Project API key: {options.ProjectApiKey}");
Console.WriteLine($"Transit name: {transitName}");

// Create a client and set to authenticate using the API key
var client = Common.CreateClient(options);

// Build a document with settings for the new transit
var createDocument = new SingleTransitLayoutCreateUpdateDocument
{
    Data = new()
    {
        Type = Common.TransitLayoutsType,
        Attributes = new()
        {
            Name = transitName,
            Frame = new()
            {
                PartNumber = S6X2Aisi316PartNumber
            },
            Drawing = new()
            {
                Revision = "A"
            },

            // The cables have two different categories. With Levelout pack algorithm,
            // they will be packed in the same frame opening. With LeveloutByCategory,
            // they will be packed in different openings.
            Cables =
            [
                new Cable {Diameter = 20, Id = "a", Category = "CatA"},
                new Cable {Diameter = 20, Id = "b", Category = "CatA"},
                new Cable {Diameter = 20, Id = "c", Category = "CatB"},
                new Cable {Diameter = 20, Id = "d", Category = "CatB"}
            ],

            PackingParameters = new()
            {
                Algorithm = LeveloutPackAlgorithm
            }
        },
        Relationships = new()
        {
            Project = Common.CreateProjectRelationship(options.ProjectId)
        }
    }
};

// Send the transit create request to the Transit Designer server
var resultDocument = await client.CreateTransitLayoutAsync(options.ProjectId, createDocument, CancellationToken.None);

Console.WriteLine("");
Console.WriteLine($"The transit was successfully created! It has transit ID {resultDocument.Data.Id}");

Console.WriteLine("");
Console.WriteLine("The complete server response was:");
Console.WriteLine(JsonConvert.SerializeObject(resultDocument, Formatting.Indented));
