using RcCloud.DateScraper.Application.Dmc.Calendar.Services;
using RcCloud.DateScraper.Application.Myrcm.Common.Domain;
using RcCloud.DateScraper.Application.Myrcm.Upcoming.Services;
using RcCloud.DateScraper.Application.Rcco;
using RcCloud.DateScraper.Application.Rck.Services;
using RcCloud.DateScraper.Domain.Races;

namespace RcCloud.DateScraper.Application.Common.Services;

public class RetrieveAllGermanRaces(
    ScrapeChallengeRaces challenge,
    ScrapeDmcRaces dmc,
    ScrapeKleinserieRaces kleinserie,
    ScrapeMyrcmRaces myrcm,
    ScrapeRcco rcco)
{
    public async Task<List<RaceMeeting>> Retrieve()
    {
        var all = (await challenge.Scrape()).ToList();

        var kleinserieAll = await kleinserie.Scrape();
        all.AddRange(kleinserieAll);

        var dmcResult = await dmc.Scrape(2026);
        if (dmcResult.IsSuccess)
        {
            all.AddRange(dmcResult.Value);
        }
        
        var myrcmAll = await myrcm.Scrape([MyrcmCountryCode.Germany]);
        all.AddRange(myrcmAll);
        
        var rccoAll = await rcco.Scrape();
        all.AddRange(rccoAll);

        all.Sort((a, b) => a.Date.CompareTo(b.Date));

        return all;
    }
}
