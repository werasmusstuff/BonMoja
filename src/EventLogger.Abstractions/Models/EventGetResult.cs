using System.Text.Json;

namespace EventLogger.Abstractions.Models
{
  public sealed class EventGetResult
  {
    public string UserId { get; set; } = default!;
    public string Timestamp { get; set; } = default!;
    public string EventId { get; set; } = default!;
    public JsonElement JsonPayload { get; set; } = default!;
  }
}
