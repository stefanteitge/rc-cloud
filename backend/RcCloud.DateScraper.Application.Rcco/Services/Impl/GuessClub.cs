using RcCloud.DateScraper.Domain.Clubs;

namespace RcCloud.DateScraper.Application.Rcco.Services.Impl;

internal class GuessClub(IClubRepository repo) : IGuessClub
{
    public Club Guess(string clubName, string? raceCountryCode)
    {
        var found = repo.FindClub(clubName);

        return new Club(
            found?.Name ?? clubName,
            found?.Aliases ?? [],
            found?.CountryCode ?? raceCountryCode,
            found?.DmcClubNumber, 
            [],
            found?.Region);
    }
}
