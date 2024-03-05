using System.Reflection;
using FluentValidation;
using MassTransit;
using StorageService.Application.Configuration;

namespace StorageService.Application;

public static class ServiceRegistrant
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        return serviceCollection.AddConsumers(configuration);
    }
    
    private static IServiceCollection AddConsumers(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        var rabbitMqOptions = GetAndValidateRabbitMqOptions(configuration);
        
        serviceCollection.AddMassTransit(configurator =>
        {
            configurator.AddConsumers(Assembly.GetEntryAssembly());

            configurator.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitMqOptions.Host, "/", h =>
                {
                    h.Username(rabbitMqOptions.Username);
                    h.Password(rabbitMqOptions.Password);
                });
                cfg.ConfigureEndpoints(context);
            });
            
        });

        return serviceCollection;
    }
    
    private static RabbitMqOptions GetAndValidateRabbitMqOptions(IConfiguration configuration)
    {
        var rabbitMqOptions = new RabbitMqOptions();
        configuration.GetSection(RabbitMqOptions.SectionName).Bind(rabbitMqOptions);

        var validator = new RabbitMqOptionsValidator();
        validator.ValidateAndThrow(rabbitMqOptions);

        return rabbitMqOptions;
    }
}