namespace EventLogger.Abstractions.AppSettings
{
  public sealed class DynamoSettings
  {
    public const string SectionName = "Dynamo";
    public string ServiceURL { get; set; } = string.Empty;
    public string Region { get; set; } = "us-west-2";

    public string AccessKey { get; set; } = string.Empty;

    public string SecretKey { get; set; } = string.Empty;

    public string TableName { get; set; } = string.Empty;
  }
}
