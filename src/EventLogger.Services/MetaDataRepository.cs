using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using EventLogger.Abstractions;
using EventLogger.Abstractions.AppSettings;
using EventLogger.Abstractions.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace EventLogger.Services
{
  public sealed class MetaDataRepository : IMetaDataRepository
  {
    private readonly string _connectionString;
    private SqlConnection CreateConnection() => new(_connectionString);

    public MetaDataRepository(IOptions<SqlSettings> sqlOptions)
    {
      _connectionString = sqlOptions.Value.ConnectionString ?? throw new ArgumentNullException(nameof(sqlOptions), "Connection string cannot be null.");
    }

    public async Task AddMetaData(EventMetadataModel eventMetadata)
    {
      await Task.Yield();

      await using var conn = CreateConnection();
      await conn.OpenAsync();

      await conn.ExecuteAsync(
          "sp_InsertEventMetadata",
          new
          {
            eventMetadata.EventId,
            eventMetadata.UserId,
            eventMetadata.EventType,
            eventMetadata.Timestamp
          },
          commandType: CommandType.StoredProcedure
      );
    }
  }
}
