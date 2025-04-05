using RcCloud.DateScraper.Domain.Clubs;
using RcCloud.DateScraper.Domain.Regions;

namespace RcCloud.DateScraper.Application.Myrcm.Upcoming.Services;

public interface IEnhanceClub
{
    Club Guess(string clubName, int myrcmClubNumber);
}
