using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RcCloud.DateScraper.Infrastructure.Clubs.Entities;
using RcCloud.DateScraper.Infrastructure.Common;
using RcCloud.DateScraper.Infrastructure.Races;

namespace RcCloud.DateScraper.Infrastructure.Clubs;

public class MongoClubRepository(
    IConfiguration configuration,
    ILogger<MongoClubRepository> logger)
    : MongoBaseRepository<ClubDbEntity>(configuration, logger)
{
    public async Task<bool> Store(ClubDbEntity clubDbEntity)
    {
        var client = GetClient();

        if (client is null)
        {
            logger.LogError("Cannot create Mongo client. Cannot store.");
            return false;
        }
        
        try
        {
            var collection = GetCollection(client, "Clubs");

            var filter = MongoDB.Driver.Builders<ClubDbEntity>.Filter.Eq(r => r.Compilation, clubDbEntity.Compilation);

            var options = new MongoDB.Driver.FindOneAndReplaceOptions<ClubDbEntity, ClubDbEntity>() { IsUpsert = true, };

            await collection.FindOneAndReplaceAsync(filter, clubDbEntity, options, CancellationToken.None);

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
