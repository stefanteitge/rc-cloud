using MongoDB.Bson;
using RcCloud.DateScraper.Domain.Races;

namespace RcCloud.DateScraper.Infrastructure.Races;

public class RacesDocument
{
    public RacesDocument(string compilation, string source, List<RaceMeeting> races, DateTimeOffset lastUpdate)
    {
        Compilation = compilation;
        Races = races;
        LastUpdate = lastUpdate;
        Source = source;
    }

    public ObjectId Id { get; set; }

    public string Compilation { get; set; }

    public List<RaceMeeting> Races { get; set; } = [];
    
    public DateTimeOffset LastUpdate { get; set; }
    public string Source { get; }
}
