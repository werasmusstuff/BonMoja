using System;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using EventLogger.Abstractions;
using EventLogger.Abstractions.AppSettings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EventLogger.Services.Extensions
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddEventLoggerServices(this IServiceCollection services, IConfiguration config)
    {
      services.AddScoped<IMetaDataRepository, MetaDataRepository>();
      services.AddScoped<IPayloadDataRepository, PayloadDataRepository>();
      services.AddScoped<IDiagnosticLogger, DiagnosticLogger>();
      services.AddScoped<IEventService, EventService>();
      services.AddScoped<IEventIdProvider, EventIdProvider>();
      services.AddScoped<IRetryMechanism, RetryMechanism>();
      services.AddScoped<IDateTimeProvider, DateTimeProvider>();

      services.Configure<SqlSettings>(config.GetSection(SqlSettings.SectionName));
      services.Configure<DynamoSettings>(config.GetSection(DynamoSettings.SectionName));

      services.AddSingleton<IAmazonDynamoDB>(sp =>
      {
        var dynamoSettings = sp.GetRequiredService<IOptions<DynamoSettings>>().Value;

        var config = new AmazonDynamoDBConfig
        {
          ServiceURL = dynamoSettings.ServiceURL,
          Timeout = TimeSpan.FromSeconds(10),
          MaxErrorRetry = 0 
        };

        var credentials = new BasicAWSCredentials(dynamoSettings.AccessKey, dynamoSettings.SecretKey);

        return new AmazonDynamoDBClient(credentials, config);
      });

      

      return services;
    }
  }
}
