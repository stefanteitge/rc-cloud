using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using RcCloud.DateScraper.Infrastructure.Races;

namespace RcCloud.DateScraper.Infrastructure.Common;

public class MongoBaseRepository(IConfiguration configuration, ILogger logger)
{
    protected MongoClient? GetClient()
    {
        var connectionUri = configuration.GetConnectionString("Mongo");

        if (string.IsNullOrEmpty(connectionUri))
        {
            logger.LogError("Mongo connection string is not set.");
            return null;
        }

        var settings = MongoClientSettings.FromConnectionString(connectionUri);
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        return new MongoClient(settings);
    }

    protected static IMongoCollection<RacesDocument> GetCollection(MongoClient client, string collection)
        => client.GetDatabase("RcCloud").GetCollection<RacesDocument>(collection);
}
