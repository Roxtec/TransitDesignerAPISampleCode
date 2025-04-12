using System;

var options = Common.ParseCommandLine(args);
var transitId = options.TransitIds.Length > 0
    ? options.TransitIds[0]
    : Common.AskTransitId();

Console.WriteLine($"Project ID: {options.ProjectId}");
Console.WriteLine($"Project API key: {options.ProjectApiKey}");
Console.WriteLine($"Transit ID: {transitId}");

// Create a client and set to authenticate using the API key
var client = Common.CreateClient(options);

// Send the delete request
await client.DeleteTransitLayoutAsync(options.ProjectId, transitId);

Console.WriteLine($"The transit with ID {transitId} was successfully deleted!");
