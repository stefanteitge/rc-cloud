using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

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
}
