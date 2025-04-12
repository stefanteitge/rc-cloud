using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using RcCloud.DateScraper.Domain.Clubs;
using RcCloud.DateScraper.Infrastructure.Common;

namespace RcCloud.DateScraper.Infrastructure.Clubs.Mongo;

public class MongoClubRepository(
    IConfiguration configuration,
    ILogger<MongoClubRepository> logger)
    : MongoBaseRepository<ClubDbDocument>(configuration, logger)
{
    // TODO: this should return something like a ClubReference domain object
    public async Task<List<Club>> GetAll(string compilation)
    {
        var client = GetClient();

        if (client is null)
        {
            logger.LogError("Cannot create Mongo client. Cannot load.");
            return [];
        }

        var collection = GetCollection(client, "Clubs");

        var filter = MongoDB.Driver.Builders<ClubDbDocument>.Filter.Eq(r => r.Compilation, compilation);
        var document = (await collection.FindAsync(filter)).FirstOrDefault();

        if (document is null)
        {
            return [];
        }
        
        return document.Clubs.Select(c => c.ToDomain()).ToList();
    }
    
    public async Task<bool> Store(List<Club> clubs)
    {
        var client = GetClient();

        if (client is null)
        {
            logger.LogError("Cannot create Mongo client. Cannot store.");
            return false;
        }
        
        try
        {
            var clubs2 = clubs.Select(c => ClubNode.FromDomain(c)).ToList();
            var document = new ClubDbDocument(clubs2, DateTimeOffset.Now, "germany");
            
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
