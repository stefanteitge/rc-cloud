using RcCloud.DateScraper.Domain.Clubs;

namespace RcCloud.DateScraper.Application.Myrcm.Upcoming.Services;

public interface IEnhanceClub
{
    Club Guess(string clubName, int myrcmClubNumber, string? countryCode);
}
