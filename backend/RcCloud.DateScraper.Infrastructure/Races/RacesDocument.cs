using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using RcCloud.DateScraper.Domain.Races;

namespace RcCloud.DateScraper.Infrastructure.Races;

internal class RacesDocument
{
    public RacesDocument(string compilation, string source, List<RaceMeeting> races, DateTimeOffset lastUpdate)
    {
        Compilation = compilation;
        Races = races;
        LastUpdate = lastUpdate.ToString();
        Source = source;
    }

    [BsonIgnoreIfDefault]
    public ObjectId Id { get; set; }

    public string Compilation { get; set; }

    // TODO: use Node class here
    public List<RaceMeeting> Races { get; set; } = [];
    
    public string LastUpdate { get; set; }

    public string Source { get; set; }
}
