namespace MessageContracts;

public record TrackingEventMessage(string? Referrer, string? UserAgent, string VisitorIpAddress) : Message;

public abstract record Message
{
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
}