using System;

namespace EventLogger.Abstractions
{
  public interface IDateTimeProvider
  {
    public DateTime UtcNow { get; }
  }
}
