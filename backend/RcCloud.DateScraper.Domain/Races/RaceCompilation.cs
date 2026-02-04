namespace RcCloud.DateScraper.Domain.Races;

public class RaceCompilation
{
    public RaceCompilation(string compilation, string source, List<RaceMeeting> races, DateTimeOffset lastUpdate)
    {
        Compilation = compilation;
        Races = races;
        LastUpdate = lastUpdate.ToString();
        Source = source;
    }
    
    public string Compilation { get; set; }

    public List<RaceMeeting> Races { get; set; } = [];
    
    public string LastUpdate { get; set; }

    public string Source { get; set; }
}
