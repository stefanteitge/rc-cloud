using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace RcCloud.DateScraper.Infrastructure.Common;

public class MongoBaseRepository(ILogger logger)
{
    protected MongoClient? GetClient()
    {
        var connectionUri = System.Environment.GetEnvironmentVariable("CONNECTION_STRING__MONGO", EnvironmentVariableTarget.Process);

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
