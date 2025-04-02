using RcCloud.DateScraper.Domain.Clubs;

namespace RcCloud.DateScraper.Application.Myrcm.Services;

public class ScrapeMyrcmClubs(ScrapeMyrcmRaces scrapeRaces)
{
    public async Task<IOrderedEnumerable<Club>> Scrape()
    {
        var races = await scrapeRaces.Scrape();

        var clubs = races
            .Select(r => r.Club)
            .OfType<Club>()
            .Distinct().OrderBy(c => c.Name);

        return clubs;
    }
}