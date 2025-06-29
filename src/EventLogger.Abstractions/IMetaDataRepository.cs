using System.Threading.Tasks;
using EventLogger.Abstractions.Models;

namespace EventLogger.Abstractions
{
  public interface IMetaDataRepository
  {
    Task AddMetaData(EventMetadataModel eventMetadata);
  }
}
