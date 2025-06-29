using System;
using EventLogger.Abstractions;

namespace EventLogger.Services
{
  public sealed class DiagnosticLogger : IDiagnosticLogger
  {
    public void Log(string message)
    {
      //Log here
      Console.WriteLine(message);
    }
  }
}
