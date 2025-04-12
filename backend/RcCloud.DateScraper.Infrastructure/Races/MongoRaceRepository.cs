using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using RcCloud.DateScraper.Domain.Races;
using RcCloud.DateScraper.Infrastructure.Common;

namespace RcCloud.DateScraper.Infrastructure.Races;

public class MongoRaceRepository(
    IConfiguration configuration,
    ILogger<MongoRaceRepository> logger)
    : MongoBaseRepository<RacesDocument>(configuration, logger)
{
    private static readonly string _collection = "Races";

    public async Task<RacesDocument?> Load(string compilation, string source)
    {
        var client = GetClient();

        if (client is null)
        {
            logger.LogError("Cannot create Mongo client. Cannot load.");
            return null;
        }

        var collection = GetCollection(client, _collection);

        return (await collection.FindAsync(MakeFilter(compilation, source))).FirstOrDefault();
    }

    public async Task<bool> Store(List<RaceMeeting> races, string compilation, string source)
    {
        var client = GetClient();

        if (client is null)
        {
            logger.LogError("Cannot create Mongo client. Cannot store.");
            return false;
        }
        
        try
        {
            var document = new RacesDocument(compilation, source, races, DateTimeOffset.Now);
            var collection = GetCollection(client, _collection);

            var filter = MakeFilter(compilation, source);

            var options = new FindOneAndReplaceOptions<RacesDocument, RacesDocument>() { IsUpsert = true, };

            await collection.FindOneAndReplaceAsync(filter, document, options, CancellationToken.None);

            logger.LogInformation("Stored races. ({Compilation}/{Source})", compilation, source);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to write clubs.");
            return false;
        }

        return true;
    }

    private static FilterDefinition<RacesDocument> MakeFilter(string compilation, string source) => Builders<RacesDocument>.Filter
                    .And(
                        Builders<RacesDocument>.Filter.Eq(r => r.Source, source),
                        Builders<RacesDocument>.Filter.Eq(r => r.Compilation, compilation));
}
