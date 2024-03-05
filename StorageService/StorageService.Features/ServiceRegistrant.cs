using Microsoft.Extensions.DependencyInjection;

namespace StorageService.Features;

public static class ServiceRegistrant
{
    public static IServiceCollection AddFeaturesServices(
        this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddMediatR(conf =>
        {
            conf.RegisterServicesFromAssemblyContaining(
                typeof(ServiceRegistrant));
        });
    }
}
