using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using RcCloud.DateScraper.Domain.Races;
using RcCloud.DateScraper.Infrastructure.Common;

namespace RcCloud.DateScraper.Infrastructure.Races;

internal class MongoRaceCompilationRepository(
    IConfiguration configuration,
    ILogger<MongoRaceCompilationRepository> logger)
    : MongoBaseRepository<RacesDocument>(configuration, logger), IRaceCompilationRepository
{
    private static readonly string _collection = "Races";

    public async Task<RaceCompilation?> Load(string compilation, string source)
    {
        var client = GetClient();

        if (client is null)
        {
            logger.LogError("Cannot create Mongo client. Cannot load.");
            return null;
        }

        var collection = GetCollection(client, _collection);

        var q = (await collection.FindAsync(MakeFilter(compilation, source))).FirstOrDefault();

        if (q is null)
        {
            return null;
        }

        return new RaceCompilation(q.Compilation, q.Compilation, q.Races, DateTimeOffset.Parse(q.LastUpdate));
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
