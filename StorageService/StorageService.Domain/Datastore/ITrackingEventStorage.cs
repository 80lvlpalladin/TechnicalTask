namespace StorageService.Domain.Datastore;

public interface ITrackingEventStorage
{
    public Task AppendAsync(TrackingEvent trackingEvent, CancellationToken cancellationToken = default);   
}

public record TrackingEvent(DateTime CreatedAt, string? Referrer, string? UserAgent, string VisitorIpAddress);