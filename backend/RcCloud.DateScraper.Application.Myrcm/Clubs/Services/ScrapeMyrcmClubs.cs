using RcCloud.DateScraper.Application.Myrcm.Common.Domain;
using RcCloud.DateScraper.Application.Myrcm.Upcoming.Services;
using RcCloud.DateScraper.Domain.Clubs;

namespace RcCloud.DateScraper.Application.Myrcm.Clubs.Services;

public class ScrapeMyrcmClubs(ScrapeMyrcmRaces scrapeRaces)
{
    public async Task<List<Club>> Scrape(MyrcmCountryCode[] countries)
    {
        var races = await scrapeRaces.Scrape(countries);

        var clubs = races
            .Select(r => r.Club)
            .OfType<Club>()
            .Distinct()
            .OrderBy(c => c.Name)
            .ToList();

        return clubs;
    }
}
