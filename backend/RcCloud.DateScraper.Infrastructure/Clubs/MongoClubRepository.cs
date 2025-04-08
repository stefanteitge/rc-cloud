using DnsClient.Internal;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using RcCloud.DateScraper.Infrastructure.Clubs.Entities;
using RcCloud.DateScraper.Infrastructure.Common;

namespace RcCloud.DateScraper.Infrastructure.Clubs;

public class MongoClubRepository(ILogger<MongoClubRepository> logger) : MongoBaseRepository(logger)
{
    public bool Store(ClubDbEntity clubDbEntity)
    {
        var client = GetClient();

        if (client is null)
        {
            logger.LogError("Cannot create Mongo client. Cannot store.");
            return false;
        }
        
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
