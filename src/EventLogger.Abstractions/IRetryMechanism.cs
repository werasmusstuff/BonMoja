using System.Threading.Tasks;

namespace EventLogger.Abstractions
{
  public interface IRetryMechanism
  {
    Task EnqueueAsync();
  }
}
