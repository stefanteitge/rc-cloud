using RcCloud.DateScraper.Domain.Clubs;

namespace RcCloud.DateScraper.Application.Myrcm.Upcoming.Services;

public interface IGuessClub
{
    Club Guess(string clubName, int myrcmClubNumber, string? raceCountryCode);
}
