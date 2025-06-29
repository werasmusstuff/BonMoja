namespace EventLogger.Abstractions.Models
{
  public sealed class EventResult
  {
    public string EventId { get; set; } = string.Empty;

    public bool SqlSuccess { get; set; }
    public bool DynamoSuccess { get; set; }

    public bool RetryQueued { get; set; }
  }
}
