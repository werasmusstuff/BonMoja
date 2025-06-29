using System.Text.Json;

namespace EventLogger.Abstractions.Models
{
  public sealed class EventRequest
  {
    public string UserId { get; set; } = null!;
    public string EventType { get; set; } = null!;
    public JsonElement JsonDetails { get; set; }
  }
}
