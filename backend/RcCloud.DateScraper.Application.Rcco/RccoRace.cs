using RcCloud.DateScraper.Domain.Races;
using RcCloud.DateScraper.Domain.Series;

namespace RcCloud.DateScraper.Application.Rcco;

public class RccoRace(string title)
{
    public string Title { get; } = title;
    
    public DateTimeOffset? Timestamp { get; set; }

    public string DateStart { get; set; }
    
    public DateOnly DateEnd { get; set; }
    
    public string Club { get; set; }
    
    public string Location { get; set; }

    public RaceMeeting ToRaceMeeting()
    {
        return new RaceMeeting([], SeasonReference.Current, DateEnd, Location, "de", Title, [], null, "rccar-online.de");
    }
}
