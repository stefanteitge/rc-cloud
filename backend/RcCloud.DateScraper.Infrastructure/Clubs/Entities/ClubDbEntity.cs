namespace RcCloud.DateScraper.Infrastructure.Clubs.Entities;

public class ClubDbEntity(List<ClubEntity> clubs, DateTimeOffset lastUpdated)
{
    public List<ClubEntity> Clubs { get; set; } = clubs;

    public DateTimeOffset LastUpdated { get; set;  } = lastUpdated;
}
