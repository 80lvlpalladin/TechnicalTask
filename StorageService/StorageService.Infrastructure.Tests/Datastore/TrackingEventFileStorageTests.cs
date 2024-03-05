using Microsoft.Extensions.Options;
using StorageService.Domain.Datastore;
using StorageService.Infrastructure.Configuration;
using StorageService.Infrastructure.Datastore;

namespace StorageService.Infrastructure.Tests.Datastore;

public class TrackingEventFileStorageTests : IDisposable
{
    private const string TempFilePath = "tmp/visits.log";

    [Fact]
    public async Task AppendAsync_WritesTrackingEvent_InTheCorrectFormat()
    {
        //Arrange
        var options = Options.Create(new TrackingEventStorageOptions { FilePath = TempFilePath });
        var storage = new TrackingEventFileStorage(options);
        var trackingEvent = new TrackingEvent(
            DateTime.Parse("2024-03-05T22:16:16.1907340Z"), 
            "http://example.com", 
            "TestUserAgent", 
            "127.0.0.1");
        var expectedFileText = $"2024-03-05T22:16:16.1907340Z | http://example.com | TestUserAgent | 127.0.0.1\n";
        
        //Act
        await storage.AppendAsync(trackingEvent, CancellationToken.None);
        
        //Assert
        var actualFileText = await File.ReadAllTextAsync(TempFilePath);
        Assert.Equal(expectedFileText, actualFileText);
    }

    [Fact]
    public async Task AppendAsync_WritesReferrerAndUserAgentFields_AsNullStrings_IfTheyAreNull()
    {
        //Arrange
        var options = Options.Create(new TrackingEventStorageOptions { FilePath = TempFilePath });
        var storage = new TrackingEventFileStorage(options);
        var trackingEvent = new TrackingEvent(
            DateTime.Parse("2024-03-05T22:16:16.1907340Z"), 
            null, 
            null, 
            "127.0.0.1");
        var expectedFileText = $"2024-03-05T22:16:16.1907340Z | null | null | 127.0.0.1\n";
        
        //Act
        await storage.AppendAsync(trackingEvent, CancellationToken.None);
        
        //Assert
        var actualFileText = await File.ReadAllTextAsync(TempFilePath);
        Assert.Equal(expectedFileText, actualFileText);
    }

    [Fact]
    public async Task AppendAsync_ThrowsArgumentNullException_IfVisitorIpAddressFieldIsNull()
    {
        //Arrange
        var options = Options.Create(new TrackingEventStorageOptions { FilePath = TempFilePath });
        var storage = new TrackingEventFileStorage(options);
        var trackingEvent = new TrackingEvent(
            DateTime.Parse("2024-03-05T22:16:16.1907340Z"), 
            "http://example.com", 
            "TestUserAgent", 
            null);
        
        //Act-Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await storage.AppendAsync(trackingEvent, CancellationToken.None));
    }


    public void Dispose()
    {
        if(File.Exists(TempFilePath))
            File.Delete(TempFilePath);
    }
}