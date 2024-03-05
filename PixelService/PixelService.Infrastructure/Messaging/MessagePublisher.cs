using MassTransit;
using MessageContracts;
using PixelService.Domain.Messaging;

namespace PixelService.Infrastructure.Messaging;

public class MessagePublisher(IPublishEndpoint publishEndpoint) : IMessagePublisher
{
    public Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : Message
    {
        ArgumentNullException.ThrowIfNull(message, nameof(message));
        return publishEndpoint.Publish(message, cancellationToken);
    }
}