using CarShop.FileService.Grpc;
using CarShop.FileService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<CatalogImageSaver>();
builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<FileServiceImpl>();

app.Run();