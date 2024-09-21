var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.CarShop_Web>("webfrontend")
    .WithExternalHttpEndpoints();

builder.AddProject<Projects.CarShop_CarStorage>("carshop-carstorage");

builder.Build().Run();
