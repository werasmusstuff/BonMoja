using System.Collections.Generic;
using System.Threading.Tasks;
using EventLogger.Abstractions.Models;

namespace EventLogger.Abstractions
{
  public interface IPayloadDataRepository
  {
    Task AddPayloadData(string userId, string eventId, string eventTime, string payloadData);

    Task<List<EventGetResult>> ReadPayloadData(string eventId);
  }
}
