using RcCloud.DateScraper.Domain.Clubs;
using RcCloud.DateScraper.Domain.Races;
using RcCloud.DateScraper.Domain.Series;

namespace RcCloud.DateScraper.Application.Rcco;

public class RccoRace(string title)
{
    public string Title { get; } = title;
    
    public DateTimeOffset? Timestamp { get; set; }

    public string DateStart { get; set; }
    
    public DateOnly DateEnd { get; set; }
    
    public string ClubName { get; set; }
    
    public string Location { get; set; }

    public RaceMeeting ToRaceMeeting(Club club)
    {
        return new RaceMeeting([], SeasonReference.Current, DateEnd, Location, "de", Title, [], club, "rccar-online.de");
    }
}
