using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StorageService.Domain.Datastore;
using StorageService.Infrastructure.Configuration;
using StorageService.Infrastructure.Datastore;

namespace StorageService.Infrastructure;

public static class ServiceRegistrant
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection serviceCollection, IConfiguration configuration)
    {
         serviceCollection
            .AddOptions<TrackingEventStorageOptions>()
            .Bind(configuration.GetSection(TrackingEventStorageOptions.SectionName));
            
         return serviceCollection.AddSingleton<ITrackingEventStorage, TrackingEventFileStorage>();
    }
}