using RcCloud.DateScraper.Domain.Clubs;

namespace RcCloud.DateScraper.Application.Myrcm.Upcoming.Services.Impl;

internal class EnhanceClub(IClubRepository repo) : IEnhanceClub
{
    public Club Guess(string clubName, int myrcmClubNumber, string? raceCountyCode)
    {
        var found = repo.FindClub(clubName);

        return new Club(
            found?.Name ?? clubName,
            found?.Aliases ?? [],
            found?.CountryCode ?? raceCountyCode,
            found?.DmcClubNumber, 
            [myrcmClubNumber],
            found?.Region);
    }
}
