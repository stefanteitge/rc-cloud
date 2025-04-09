using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using RcCloud.DateScraper.Domain.Races;
using RcCloud.DateScraper.Infrastructure.Common;

namespace RcCloud.DateScraper.Infrastructure.Races;

public class MongoRaceRepository(ILogger<MongoRaceRepository> logger) : MongoBaseRepository(logger)
{
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
            var collection = client
                .GetDatabase("RcCloud").GetCollection<RacesDocument>("Races");

            var filter = Builders<RacesDocument>.Filter
                .And(
                    Builders<RacesDocument>.Filter.Eq(r => r.Source, source),
                    Builders<RacesDocument>.Filter.Eq(r => r.Compilation, compilation));

            var options = new FindOneAndReplaceOptions<RacesDocument, RacesDocument>() { IsUpsert = true, };
            
            await collection.FindOneAndReplaceAsync(filter, document, options, CancellationToken.None);

            logger.LogInformation("Stored clubs. ({Compilation}/{Source})", compilation, source);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to write clubs.");
            return false;
        }

        return true;
    }
}
