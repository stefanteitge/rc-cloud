using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using RcCloud.DateScraper.Infrastructure.Common;

namespace RcCloud.DateScraper.Infrastructure.Clubs.Mongo;

public class MongoClubRepository(
    IConfiguration configuration,
    ILogger<MongoClubRepository> logger)
    : MongoBaseRepository<ClubDbDocument>(configuration, logger)
{
    public async Task<ClubDbDocument?> GetAll(string compilation)
    {
        var client = GetClient();

        if (client is null)
        {
            logger.LogError("Cannot create Mongo client. Cannot load.");
            return null;
        }

        var collection = GetCollection(client, "Clubs");

        var filter = MongoDB.Driver.Builders<ClubDbDocument>.Filter.Eq(r => r.Compilation, compilation);
        return (await collection.FindAsync(filter)).FirstOrDefault();
    }
    
    public async Task<bool> Store(ClubDbDocument document)
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

            var filter = MongoDB.Driver.Builders<ClubDbDocument>.Filter.Eq(r => r.Compilation, document.Compilation);

            var options = new MongoDB.Driver.FindOneAndReplaceOptions<ClubDbDocument, ClubDbDocument>() { IsUpsert = true, };

            await collection.FindOneAndReplaceAsync(filter, document, options, CancellationToken.None);

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
