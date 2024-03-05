using MediatR;
using PixelService.Features;
using PixelService.Features.Track;
using PixelService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile($"appsettings.json");

builder.Services
    .AddInfrastructureServices(builder.Configuration)
    .AddFeaturesServices();


var app = builder.Build();

app.MapGet("/track", async (IMediator mediator, HttpContext httpContext) =>
{
    var referrer = httpContext.Request.Headers["Referrer"].ToString();
    var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
    var visitorIp = httpContext.Connection.RemoteIpAddress?.ToString();
    
    if(visitorIp is null)
        return Results.BadRequest("Could not determine visitor IP.");
    
    var errorOrResult = await mediator.Send(new TrackRequest(referrer, userAgent, visitorIp));

    return errorOrResult.MatchFirst(
        value => Results.File(value.ImageData, "image/gif"), 
        error => Results.BadRequest(error.Description));
});

app.Run();