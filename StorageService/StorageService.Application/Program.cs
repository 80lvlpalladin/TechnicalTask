using StorageService.Application;
using StorageService.Features;
using StorageService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json");

builder.Services
    .AddInfrastructureServices(builder.Configuration)
    .AddFeaturesServices()
    .AddApplicationServices(builder.Configuration);

var app = builder.Build();
app.Run();