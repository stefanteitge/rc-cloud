using RcCloud.DateScraper.Application.Myrcm.Upcoming.Services;
using RcCloud.DateScraper.Domain.Clubs;

namespace RcCloud.DateScraper.Application.Myrcm.Clubs.Services;

public class ScrapeMyrcmClubs(ScrapeMyrcmRaces scrapeRaces, SanitizeClubNames sanitizeClubNames)
{
    public async Task<List<Club>> Scrape()
    {
        var races = await scrapeRaces.Scrape();

        var clubs = races
            .Select(r => r.Club)
            .OfType<Club>()
            .Distinct()
            .OrderBy(c => c.Name)
            .ToList();

        sanitizeClubNames.Sanitize(clubs);

        return clubs;
    }
}