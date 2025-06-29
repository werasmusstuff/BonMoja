using System.Text.Json;
using EventLogger.Abstractions;
using EventLogger.Abstractions.Models;
using EventLogger.Services;
using NSubstitute;
using FluentAssertions;

namespace EventLogger.Tests
{
  public class EventServiceTests
  {
    [Fact]
    public async Task RecordEventAsync_ShouldReturnSuccess_WhenEventIsRecorded()
    {
      // Arrange

      var metaDataRepositoryMock = Substitute.For<IMetaDataRepository>();
      var payloadDataRepositoryMock = Substitute.For<IPayloadDataRepository>();
      var eventIdProviderMock = Substitute.For<IEventIdProvider>();
      var retryMechanismMock = Substitute.For<IRetryMechanism>();
      var dateTimeProviderMock = Substitute.For<IDateTimeProvider>();

      var eventId = "34ed0f1d-1cff-4abe-94a0-04fc7ba13c9a";
      eventIdProviderMock.GenerateEventIdAsync().Returns(eventId);

      var SUT = new EventService(
        metaDataRepositoryMock,
        payloadDataRepositoryMock,
        eventIdProviderMock,
        retryMechanismMock,
        dateTimeProviderMock
      );

      var request = new EventRequest
      {
        UserId = "12345",
        EventType = "test",
        JsonDetails = JsonSerializer.SerializeToElement(new { value = 42 })
      };

      // Act
      var response = await SUT.RecordEventAsync(request);


      // Assert
      response.Should().NotBeNull();
      response.EventId.Should().Be(eventId);
      response.SqlSuccess.Should().BeTrue();
      response.DynamoSuccess.Should().BeTrue();
      response.RetryQueued.Should().BeFalse();

      await metaDataRepositoryMock.Received(1).AddMetaData(Arg.Any<EventMetadataModel>());
      await payloadDataRepositoryMock.Received(1).AddPayloadData(Arg.Any<string>(), Arg.Is(eventId), Arg.Any<string>(), Arg.Any<string>());

      // Verify retry queue was NOT triggered
      await retryMechanismMock.DidNotReceiveWithAnyArgs().EnqueueAsync();

    }

    [Fact]
    public async Task RecordEventAsync_ShouldReturnFailureAndQueueRetry_WhenMetaDataRepositoryThrows()
    {
      // Arrange
      var metaDataRepositoryMock = Substitute.For<IMetaDataRepository>();
      var payloadDataRepositoryMock = Substitute.For<IPayloadDataRepository>();
      var eventIdProviderMock = Substitute.For<IEventIdProvider>();
      var retryMechanismMock = Substitute.For<IRetryMechanism>();
      var dateTimeProviderMock = Substitute.For<IDateTimeProvider>();

      var eventId = "34ed0f1d-1cff-4abe-94a0-04fc7ba13c9a";
      eventIdProviderMock.GenerateEventIdAsync().Returns(eventId);

      // Setup metadata repo to throw an exception when AddMetaData is called
      metaDataRepositoryMock
       .AddMetaData(Arg.Any<EventMetadataModel>())
       .Returns<Task>(x => Task.FromException(new Exception("SQL failure")));

      var sut = new EventService(
          metaDataRepositoryMock,
          payloadDataRepositoryMock,
          eventIdProviderMock,
          retryMechanismMock,
          dateTimeProviderMock
      );

      var request = new EventRequest
      {
        UserId = "12345",
        EventType = "test",
        JsonDetails = JsonSerializer.SerializeToElement(new { value = 42 })
      };

      // Act
      var response = await sut.RecordEventAsync(request);

      // Assert
      response.Should().NotBeNull();
      response.EventId.Should().Be(eventId);
      response.SqlSuccess.Should().BeFalse();         
      response.DynamoSuccess.Should().BeTrue();       
      response.RetryQueued.Should().BeTrue();

      await metaDataRepositoryMock.Received(1).AddMetaData(Arg.Any<EventMetadataModel>());

      await payloadDataRepositoryMock.Received(1).AddPayloadData(Arg.Any<string>(), Arg.Is(eventId), Arg.Any<string>(), Arg.Any<string>());

      await retryMechanismMock.Received(1).EnqueueAsync();
    }

    [Fact]
    public async Task RecordEventAsync_ShouldReturnFailureAndQueueRetry_WhenPayloadRepositoryThrows()
    {
      // Arrange
      var metaDataRepositoryMock = Substitute.For<IMetaDataRepository>();
      var payloadDataRepositoryMock = Substitute.For<IPayloadDataRepository>();
      var eventIdProviderMock = Substitute.For<IEventIdProvider>();
      var retryMechanismMock = Substitute.For<IRetryMechanism>();
      var dateTimeProviderMock = Substitute.For<IDateTimeProvider>();

      var eventId = "34ed0f1d-1cff-4abe-94a0-04fc7ba13c9a";
      eventIdProviderMock.GenerateEventIdAsync().Returns(eventId);

      // Setup metadata repo to throw an exception when AddMetaData is called
      payloadDataRepositoryMock
       .AddPayloadData(Arg.Any<string>(), Arg.Is(eventId), Arg.Any<string>(), Arg.Any<string>())
       .Returns<Task>(x => Task.FromException(new Exception("SQL failure")));

      var sut = new EventService(
          metaDataRepositoryMock,
          payloadDataRepositoryMock,
          eventIdProviderMock,
          retryMechanismMock,
          dateTimeProviderMock
      );

      var request = new EventRequest
      {
        UserId = "12345",
        EventType = "test",
        JsonDetails = JsonSerializer.SerializeToElement(new { value = 42 })
      };

      // Act
      var response = await sut.RecordEventAsync(request);

      // Assert
      response.Should().NotBeNull();
      response.EventId.Should().Be(eventId);
      response.SqlSuccess.Should().BeTrue();
      response.DynamoSuccess.Should().BeFalse();
      response.RetryQueued.Should().BeTrue();

      await metaDataRepositoryMock.Received(1).AddMetaData(Arg.Any<EventMetadataModel>());

      await payloadDataRepositoryMock.Received(1).AddPayloadData(Arg.Any<string>(), Arg.Is(eventId), Arg.Any<string>(), Arg.Any<string>());

      await retryMechanismMock.Received(1).EnqueueAsync();
    }
  }
}
