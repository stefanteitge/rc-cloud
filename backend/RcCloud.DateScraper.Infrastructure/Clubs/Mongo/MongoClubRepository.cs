using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using RcCloud.DateScraper.Domain.Clubs;
using RcCloud.DateScraper.Infrastructure.Common;

namespace RcCloud.DateScraper.Infrastructure.Clubs.Mongo;

internal class MongoClubRepository(
    IMongoClient client,
    ILogger<MongoClubRepository> logger)
    : MongoBaseRepository<ClubDbDocument>(), IClubCopyRepository
{
    // TODO: this should return something like a ClubReference domain object
    public async Task<List<Club>> GetAll(string compilation)
    {
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
