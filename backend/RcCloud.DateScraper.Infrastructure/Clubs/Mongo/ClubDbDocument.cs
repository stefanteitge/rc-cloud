using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using RcCloud.DateScraper.Infrastructure.Clubs.Entities;

namespace RcCloud.DateScraper.Infrastructure.Clubs.Mongo;

public class ClubDbDocument(List<ClubJson> clubs, DateTimeOffset lastUpdated, string compilation)
{
    [BsonIgnoreIfDefault]
    public ObjectId Id { get; set; }
    
    public string Compilation { get; set; } = compilation;
    
    public List<ClubJson> Clubs { get; set; } = clubs;

    public DateTimeOffset LastUpdated { get; set;  } = lastUpdated;
}
