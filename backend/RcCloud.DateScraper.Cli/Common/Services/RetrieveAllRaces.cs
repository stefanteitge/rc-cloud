using RcCloud.DateScraper.Application.Dmc.Calendar.Services;
using RcCloud.DateScraper.Application.Myrcm.Common.Domain;
using RcCloud.DateScraper.Application.Myrcm.Upcoming.Services;
using RcCloud.DateScraper.Application.Rcco;
using RcCloud.DateScraper.Application.Rck.Services;
using RcCloud.DateScraper.Domain.Races;

namespace RcCloud.DateScraper.Cli.Common.Services;

public class RetrieveAllRaces(
    ScrapeChallengeRaces scrapeChallenge,
    ScrapeDmcRaces scrapeDmc,
    ScrapeKleinserieRaces scrapeKleinserie,
    ScrapeMyrcmRaces myrcm,
    ScrapeRcco rcco)
{
    public async Task<List<RaceMeeting>> Retrieve()
    {
        var all = (await scrapeChallenge.Parse()).ToList();

        var kleinserieAll = await scrapeKleinserie.Parse();
        all.AddRange(kleinserieAll);

        var dmcAll = await scrapeDmc.Parse();
        all.AddRange(dmcAll);
        
        var myrcmAll = await myrcm.Scrape([MyrcmCountryCode.Germany]);
        all.AddRange(myrcmAll);
        
        var rccoAll = await rcco.Scrape();
        all.AddRange(rccoAll);

        all.Sort((a, b) => a.Date.CompareTo(b.Date));

        return all;
    }
}
