using ErrorOr;
using MessageContracts;
using Moq;
using PixelService.Domain.Messaging;
using PixelService.Features.Track;

namespace PixelService.Features.Tests.Track;

public class TrackHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsErrorValidation_IfVisitorIpAddressIsNullOrWhitespace()
    {
        // Arrange
        var messagePublisher = new Mock<IMessagePublisher>();
        var handler = new TrackHandler(messagePublisher.Object);
        TrackRequest[] invalidRequests =
        [
            new TrackRequest(null, null, null!),
            new TrackRequest(null, null, string.Empty),
            new TrackRequest(null, null, " ")
        ];
        
        foreach (var request in invalidRequests)
        {
            // Act
            var errorOrResult = await handler.Handle(request, CancellationToken.None);

            //Assert
            Assert.True(errorOrResult.IsError);
            Assert.Single(errorOrResult.Errors);
            Assert.Equal(ErrorType.Validation, errorOrResult.FirstError.Type);
        }
        
        //Final Assert
        Assert.Equal(0, messagePublisher.Invocations.Count);
    }

    [Fact]
    public async Task Handle_PublishesTrackingInfoMessage_ToMessagePublisher()
    {
        // Arrange
        var trackRequest = new TrackRequest("test referrer", "test user agent", "test ip address");
        var messagePublisher = new Mock<IMessagePublisher>(MockBehavior.Strict);
        messagePublisher
            .Setup(publisher => 
                publisher.PublishAsync(It.Is<TrackingEventMessage>(message => 
                    message.Referrer == trackRequest.Referrer && 
                    message.UserAgent == trackRequest.UserAgent && 
                    message.VisitorIpAddress == trackRequest.VisitorIpAddress), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var handler = new TrackHandler(messagePublisher.Object);
        
        // Act
        var errorOrResult = await handler.Handle(trackRequest, CancellationToken.None);
        
        //Assert
        Assert.False(errorOrResult.IsError);
        Assert.Equal(1, messagePublisher.Invocations.Count);
    }
}