namespace RcCloud.DateScraper.Infrastructure.Common.Configuration;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string AuthSource { get; set; } = "admin";
}
