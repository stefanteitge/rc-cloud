namespace RcCloud.DateScraper.Infrastructure.Clubs.Json;

public class ClubDbJsonRoot(List<ClubJson> clubs, DateTimeOffset lastUpdated, string compilation)
{
    public string Compilation { get; set; } = compilation;
    
    public List<ClubJson> Clubs { get; set; } = clubs;

    public DateTimeOffset LastUpdated { get; set;  } = lastUpdated;
}
