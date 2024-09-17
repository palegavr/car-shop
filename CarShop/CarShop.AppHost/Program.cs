var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.CarShop_Web>("webfrontend")
    .WithExternalHttpEndpoints();

builder.Build().Run();
