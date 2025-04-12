using System;
using Newtonsoft.Json;

var options = Common.ParseCommandLine(args);
var transitId = options.TransitIds.Length > 0
    ? options.TransitIds[0]
    : Common.AskTransitId();

Console.WriteLine($"Project ID: {options.ProjectId}");
Console.WriteLine($"Project API key: {options.ProjectApiKey}");
Console.WriteLine($"Transit ID: {transitId}");

// Create a client and set to authenticate using the API key
var client = Common.CreateClient(options);

// Request using project ID and transit ID
var resultDocument = await client.GetTransitLayoutAsync(options.ProjectId, transitId);

Console.WriteLine("");
Console.WriteLine("The complete server response was:");
Console.WriteLine(JsonConvert.SerializeObject(resultDocument, Formatting.Indented));
