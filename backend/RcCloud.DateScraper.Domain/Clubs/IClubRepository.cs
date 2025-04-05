using RcCloud.DateScraper.Domain.Regions;

namespace RcCloud.DateScraper.Domain.Clubs;

public interface IClubRepository
{
    Club? FindClub(string clubName);
}
