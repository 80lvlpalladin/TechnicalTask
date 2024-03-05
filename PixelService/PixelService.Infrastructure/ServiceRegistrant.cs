using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PixelService.Domain.Messaging;
using PixelService.Infrastructure.Configuration;
using PixelService.Infrastructure.Datastore;
using PixelService.Infrastructure.Messaging;

namespace PixelService.Infrastructure;

public static class ServiceRegistrant
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection serviceCollection, IConfiguration configuration) =>
        serviceCollection
            .AddMassTransit(configuration)
            .AddOutbox(configuration)
            .AddScoped<IMessagePublisher, MessagePublisher>();

    private static IServiceCollection AddMassTransit(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        var rabbitMqOptions = GetAndValidateRabbitMqOptions(configuration);
        
        serviceCollection.AddMassTransit(configurator =>
        {
            configurator.AddEntityFrameworkOutbox<OutboxDbContext>(c =>
            {
                c.UseMySql();
                c.UseBusOutbox();
                c.DisableInboxCleanupService();
            });
            
            configurator.UsingRabbitMq((_, cfg) =>
            {
                cfg.Host(rabbitMqOptions.Host, "/", h =>
                {
                    h.Username(rabbitMqOptions.Username);
                    h.Password(rabbitMqOptions.Password);
                });

            });
            
        });

        return serviceCollection;
    }

    private static IServiceCollection AddOutbox(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        return serviceCollection.AddDbContext<OutboxDbContext>(options =>
        {
            var paymentsDbConnectionString =
                configuration.GetConnectionString("OutboxDatabase");
            
            ArgumentException.ThrowIfNullOrEmpty(paymentsDbConnectionString);

            options.UseMySql(paymentsDbConnectionString,
                ServerVersion.AutoDetect(paymentsDbConnectionString));
        });
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
