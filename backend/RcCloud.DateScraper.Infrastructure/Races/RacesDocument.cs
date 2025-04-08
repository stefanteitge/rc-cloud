using RcCloud.DateScraper.Domain.Races;

namespace RcCloud.DateScraper.Infrastructure.Races;

public class RacesDocument(string compilation, List<RaceMeeting> races, DateTimeOffset lastUpdate)
{
    public string Compilation { get; } = compilation;
    
    public List<RaceMeeting> Races { get; } = races;
    
    public DateTimeOffset LastUpdate { get; } = lastUpdate;
}
