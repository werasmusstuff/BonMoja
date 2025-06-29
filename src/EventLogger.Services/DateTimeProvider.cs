using System;
using EventLogger.Abstractions;

namespace EventLogger.Services
{
  public sealed class DateTimeProvider : IDateTimeProvider
  {
    public DateTime UtcNow => DateTime.UtcNow;
  }
}
