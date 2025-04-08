using DnsClient.Internal;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using RcCloud.DateScraper.Infrastructure.Clubs.Entities;

namespace RcCloud.DateScraper.Infrastructure.Clubs;

public class MongoClubRepository(ILogger<MongoClubRepository> logger)
{
    public bool Store(ClubDbEntity clubDbEntity)
    {
        var connectionUri = System.Environment.GetEnvironmentVariable("CONNECTION_STRING__MONGO", EnvironmentVariableTarget.Process);

        if (string.IsNullOrEmpty(connectionUri))
        {
            logger.LogError("Mongo connection string is not set.");
            return false;
        }

        var settings = MongoClientSettings.FromConnectionString(connectionUri);
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        var client = new MongoClient(settings);
        try
        {
            var collection = client
                .GetDatabase("RcCloud").GetCollection<ClubDbEntity>("Clubs"); ;
            collection.InsertOne(clubDbEntity);

            logger.LogInformation("Stored clubs.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to write clubs.");
            return false;
        }

        return true;
    }
}
