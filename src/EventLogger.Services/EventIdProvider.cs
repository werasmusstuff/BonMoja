using System;
using EventLogger.Abstractions;

namespace EventLogger.Services
{
  public sealed class EventIdProvider : IEventIdProvider
  {
    public string GenerateEventIdAsync()
    {
      return Guid.NewGuid().ToString();
    }
  }
}
