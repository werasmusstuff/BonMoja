using System.Threading.Tasks;
using EventLogger.Abstractions;

namespace EventLogger.Services
{
  public sealed class RetryMechanism : IRetryMechanism
  {
    public Task EnqueueAsync()
    {
      //no implementation for retries exist as off yet, time constraints.
      //You could retry here with limits. If it still fails push to a retry queue.
      return Task.CompletedTask;
    }
  }
}
