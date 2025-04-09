using RcCloud.DateScraper.Domain.Clubs;

namespace RcCloud.DateScraper.Application.Myrcm.Upcoming.Services.Impl;

internal class EnhanceClub(IClubRepository repo) : IEnhanceClub
{
    public Club Guess(string clubName, int myrcmClubNumber)
    {
        var found = repo.FindClub(clubName);

        return new Club(found?.Name ?? clubName, found?.Aliases ?? [], null, [myrcmClubNumber], found?.Region);
    }
}
