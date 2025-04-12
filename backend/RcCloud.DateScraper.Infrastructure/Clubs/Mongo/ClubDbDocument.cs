using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using RcCloud.DateScraper.Infrastructure.Clubs.Json;

namespace RcCloud.DateScraper.Infrastructure.Clubs.Mongo;

public class ClubDbDocument(List<ClubNode> clubs, DateTimeOffset lastUpdated, string compilation)
{
    [BsonIgnoreIfDefault]
    public ObjectId Id { get; set; }
    
    public string Compilation { get; set; } = compilation;
    
    public List<ClubNode> Clubs { get; set; } = clubs;

    public DateTimeOffset LastUpdated { get; set;  } = lastUpdated;
}
