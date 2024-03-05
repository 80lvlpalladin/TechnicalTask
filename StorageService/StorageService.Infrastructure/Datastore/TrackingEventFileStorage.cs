using System.Globalization;
using Microsoft.Extensions.Options;
using StorageService.Domain.Datastore;
using StorageService.Infrastructure.Configuration;

namespace StorageService.Infrastructure.Datastore;

public class TrackingEventFileStorage(IOptions<TrackingEventStorageOptions> options) : ITrackingEventStorage
{
    private static readonly SemaphoreSlim _semaphore = new(1, 1);
    
    public async Task AppendAsync(TrackingEvent trackingEvent, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(trackingEvent.VisitorIpAddress, nameof(trackingEvent.CreatedAt));
        
        var iso8601Date = trackingEvent.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ", CultureInfo.InvariantCulture);
        var referrer = string.IsNullOrWhiteSpace(trackingEvent.Referrer) ? "null" : trackingEvent.Referrer;
        var userAgent = string.IsNullOrWhiteSpace(trackingEvent.UserAgent) ? "null" : trackingEvent.UserAgent;
        
        var stringToAppend = $"{iso8601Date} | {referrer} | {userAgent} | {trackingEvent.VisitorIpAddress}";

        await _semaphore.WaitAsync(cancellationToken);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(options.Value.FilePath) ?? string.Empty);
            await File.AppendAllTextAsync(options.Value.FilePath, $"{stringToAppend}\n", cancellationToken);
        }
        finally
        {
            _semaphore.Release();
        }
    }
}