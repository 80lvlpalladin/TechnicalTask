using ErrorOr;
using MediatR;
using MessageContracts;
using PixelService.Domain.Messaging;

namespace PixelService.Features.Track;

public record TrackRequest(string? Referrer, string? UserAgent, string VisitorIpAddress) : IRequest<ErrorOr<TrackResponse>>;
public record TrackResponse(byte[] ImageData);

public class TrackHandler(IMessagePublisher messagePublisher) : IRequestHandler<TrackRequest, ErrorOr<TrackResponse>>
{
    private readonly byte[] _onePixelTransparentImageBytes =
    [
        0x47, 0x49, 0x46, 0x38, 0x39, 0x61, 0x01, 0x00,
        0x01, 0x00, 0x80, 0xff, 0x00, 0xff, 0xff, 0xff,
        0x00, 0x00, 0x00, 0x2c, 0x00, 0x00, 0x00, 0x00,
        0x01, 0x00, 0x01, 0x00, 0x00, 0x02, 0x02, 0x44,
        0x01, 0x00, 0x3b
    ];

    public async Task<ErrorOr<TrackResponse>> Handle(TrackRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.VisitorIpAddress))
            return Error.Validation("The IP address is the only mandatory value.");
        
        var message = 
            new TrackingEventMessage(request.Referrer, request.UserAgent, request.VisitorIpAddress);

        await messagePublisher.PublishAsync(message, cancellationToken);

        return new TrackResponse(_onePixelTransparentImageBytes);
    }
}