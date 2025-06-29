namespace EventLogger.Abstractions
{
  public interface IEventIdProvider
  {
    string GenerateEventIdAsync();
  }
}
