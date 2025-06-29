using Amazon.DynamoDBv2.DataModel;

namespace EventLogger.Abstractions.Models
{
  public sealed class EventPayloadModel
  {
    [DynamoDBHashKey]
    public string EventId { get; set; } = null!;

    [DynamoDBProperty]
    public string JsonPayload { get; set; } = null!;
  }
}
