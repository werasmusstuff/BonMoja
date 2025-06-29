using EventLogger.Abstractions.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventLogger.Abstractions
{
  public interface IEventService
  {
    Task<EventResult> RecordEventAsync(EventRequest request);

    Task<List<EventGetResult>> GetEventsByUserAsync(string userId, int page, int pageSize);
  }
}
