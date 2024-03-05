using MassTransit;
using MediatR;
using MessageContracts;
using StorageService.Features.ConsumeTrackingEventMessage;

namespace StorageService.Application.Consumers;

public class TrackingEventMessageConsumer(IMediator mediator) : IConsumer<TrackingEventMessage>
{
    public Task Consume(ConsumeContext<TrackingEventMessage> context)
    {
        var mediatorRequest = new ConsumeTrackingEventMessageRequest(context.Message);
        return mediator.Send(mediatorRequest, context.CancellationToken);
    }
}