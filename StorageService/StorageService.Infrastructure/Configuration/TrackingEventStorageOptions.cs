namespace StorageService.Infrastructure.Configuration;

public class TrackingEventStorageOptions
{
    public static string SectionName => "TrackingEventStorage";
    public required string FilePath { get; init; } = "tmp/visits.log";
}