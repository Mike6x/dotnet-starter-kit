var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Playground_Api>("playground-api");

await builder.Build().RunAsync();
