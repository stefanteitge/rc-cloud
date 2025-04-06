using RcCloud.DateScraper.Domain.Clubs;

namespace RcCloud.DateScraper.Application.Dmc.Calendar.Services;

public class ScrapeDmcClubs(ScrapeDmcRaces scrapeDmcRaces)
{
    public async Task<List<Club>> Scrape()
    {
        var races = await scrapeDmcRaces.Parse();

        return races.Select(r => r.Club).OfType<Club>().ToList();
    }
}
