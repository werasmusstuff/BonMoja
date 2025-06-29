namespace EventLogger.Abstractions.Models
{
  public sealed class EventMetadataModel
  {
    public string EventId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string Timestamp { get; set; }
  }
}
