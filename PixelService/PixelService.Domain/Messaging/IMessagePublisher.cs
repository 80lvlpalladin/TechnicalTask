using MessageContracts;

namespace PixelService.Domain.Messaging;

public interface IMessagePublisher
{
    public Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : Message;
}