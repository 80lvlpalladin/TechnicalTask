using ErrorOr;
using MediatR;
using MessageContracts;
using StorageService.Domain.Datastore;

namespace StorageService.Features.ConsumeTrackingEventMessage;

public record ConsumeTrackingEventMessageRequest(TrackingEventMessage Message) : IRequest<Success>;

public class ConsumeTrackingEventMessageHandler(ITrackingEventStorage trackingEventStorage) : 
    IRequestHandler<ConsumeTrackingEventMessageRequest, Success>
{
    public async Task<Success> Handle(ConsumeTrackingEventMessageRequest request, CancellationToken cancellationToken)
    {
        var incomingMessage = request.Message;
        var domainTrackingEvent = new TrackingEvent(
            incomingMessage.CreatedAt,
            incomingMessage.Referrer,
            incomingMessage.UserAgent,
            incomingMessage.VisitorIpAddress);
        
        await trackingEventStorage.AppendAsync(domainTrackingEvent, cancellationToken);

        return new Success();
    }
}