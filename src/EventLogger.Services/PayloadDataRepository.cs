using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using EventLogger.Abstractions;
using EventLogger.Abstractions.AppSettings;
using EventLogger.Abstractions.Models;
using Microsoft.Extensions.Options;

namespace EventLogger.Services
{
  public sealed class PayloadDataRepository : IPayloadDataRepository
  {
    private readonly IAmazonDynamoDB _amazonDynamoDB;
    private readonly string _tableName;

    public PayloadDataRepository(IAmazonDynamoDB amazonDynamoDB, IOptions<DynamoSettings> dynamoSettings)
    {
      _amazonDynamoDB = amazonDynamoDB;
      _tableName = dynamoSettings.Value.TableName;
    }

    public async Task AddPayloadData(string userId, string eventId, string eventTime, string payloadData)
    {
      var item = new Dictionary<string, AttributeValue>
      {
        { "UserId", new AttributeValue { S = userId } },
        { "Timestamp", new AttributeValue { S = eventTime} },
        { "EventId", new AttributeValue { S = eventId } },
        { "JsonPayload", new AttributeValue { S = payloadData } }
      };

      var putRequest = new PutItemRequest
      {
        TableName = _tableName,
        Item = item
      };

      await _amazonDynamoDB.PutItemAsync(putRequest);
    }

    public async Task<List<EventGetResult>> ReadPayloadData(string userId)
    {
      var request = new QueryRequest
      {
        TableName = _tableName,
        KeyConditionExpression = "UserId = :uid",
        ExpressionAttributeValues = new Dictionary<string, AttributeValue>
        {
            { ":uid", new AttributeValue { S = userId } }
        },
        ScanIndexForward = true
      };

      var response = await _amazonDynamoDB.QueryAsync(request);

      var results = new List<EventGetResult>();

      foreach (var item in response.Items)
      {
        results.Add(new EventGetResult
        {
          UserId = item.TryGetValue("UserId", out var uid) ? uid.S : string.Empty,
          Timestamp = item.TryGetValue("Timestamp", out var ts) ? ts.S : string.Empty,
          EventId = item.TryGetValue("EventId", out var eid) ? eid.S : string.Empty,
          JsonPayload = item.TryGetValue("JsonPayload", out var payload) && !string.IsNullOrWhiteSpace(payload.S)
            ? JsonSerializer.Deserialize<JsonElement>(payload.S)
            : default
        });
      }

      return results;
    }
  }
}
