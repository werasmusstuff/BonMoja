namespace EventLogger.Abstractions.AppSettings
{
  public sealed class SqlSettings
  {
    public const string SectionName = "Sql";
    public string ConnectionString { get; set; } = string.Empty;
  }
}
