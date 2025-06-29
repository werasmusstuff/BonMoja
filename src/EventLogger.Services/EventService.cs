using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventLogger.Abstractions.Models;
using EventLogger.Abstractions;
using System.Text.Json;
using Microsoft.Data.SqlClient;

namespace EventLogger.Services
{
  public sealed class EventService : IEventService
  {
    private readonly IMetaDataRepository _metaDataRepository;
    private readonly IPayloadDataRepository _payloadDataRepository;
    private readonly IEventIdProvider _eventIdProvider;
    private readonly IRetryMechanism _retryMechanism;
    private readonly IDateTimeProvider _dateTimeProvider;

    public EventService(IMetaDataRepository metaDataRepository, IPayloadDataRepository payloadDataRepository, IEventIdProvider eventIdProvider, IRetryMechanism retryMechanism, IDateTimeProvider dateTimeProvider)
    {
      _metaDataRepository = metaDataRepository;
      _payloadDataRepository = payloadDataRepository;
      _eventIdProvider = eventIdProvider;
      _retryMechanism = retryMechanism;
      _dateTimeProvider = dateTimeProvider;
    }

    public async Task<EventResult> RecordEventAsync(EventRequest request)
    {
      bool sqlSuccess = true;
      bool dynamoSuccess = true;
      var eventId = _eventIdProvider.GenerateEventIdAsync();
      var eventTime = _dateTimeProvider.UtcNow.ToString("o");

      var sqlTask = _metaDataRepository.AddMetaData(buildMetaDataModel(request, eventId, eventTime));
      var dynamoTask = _payloadDataRepository.AddPayloadData(request.UserId, eventId.ToString(), eventTime, JsonSerializer.Serialize(request.JsonDetails));

      try
      {
        await sqlTask;
      }
      catch (Exception ex)
      {
        sqlSuccess = false;
        //log something here
      }

      try
      {
        await dynamoTask;
      }
      catch (Exception ex)
      {
        dynamoSuccess = false;
        //log something here
      }

      if (!sqlSuccess || !dynamoSuccess)
      {
        //build up an object here to define retries for either the SQL, the Dynamo or both.
        //this will get enqueued for retries to ensure data consistency. Currently this implementation does nothing due to time constraints, it just returns Task.CompletedTask.
        await _retryMechanism.EnqueueAsync();
      }

      return BuildResult(eventId, sqlSuccess, dynamoSuccess);
    }

    public async Task<List<EventGetResult>> GetEventsByUserAsync(string userId, int page, int pageSize)
    {
      return await _payloadDataRepository.ReadPayloadData(userId);
    }

    private EventMetadataModel buildMetaDataModel(EventRequest request, string eventId, string eventTime)
    {
      return new EventMetadataModel
      {
        EventId = eventId,
        UserId = request.UserId,
        EventType = request.EventType,
        Timestamp = eventTime
      };
    }

    private EventResult BuildResult(string eventId, bool metaDataSuccess, bool payloadDataSuccess) => new EventResult
    {
      EventId = eventId,
      SqlSuccess = metaDataSuccess,
      DynamoSuccess = payloadDataSuccess,
      RetryQueued = !(metaDataSuccess && payloadDataSuccess)
    };
  }
}
