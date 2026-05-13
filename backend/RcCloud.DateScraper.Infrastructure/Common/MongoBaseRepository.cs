using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using RcCloud.DateScraper.Infrastructure.Races;

namespace RcCloud.DateScraper.Infrastructure.Common;

public class MongoBaseRepository<T>()
{
    protected static IMongoCollection<T> GetCollection(IMongoClient client, string collection)
        => client.GetDatabase("RcCloud").GetCollection<T>(collection);
}
