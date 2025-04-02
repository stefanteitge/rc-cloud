using RcCloud.DateScraper.Application.Dmc;
using RcCloud.DateScraper.Application.Myrcm.Services;
using RcCloud.DateScraper.Application.Rck.Services;
using RcCloud.DateScraper.Domain;
using RcCloud.DateScraper.Domain.Races;

namespace RcCloud.DateScraper.Cli.Commands.Utils;

public class RetrieveAllService(
    ChallengeService challenge,
    ScrapeDmcRaces scrapeDmc,
    KleinserieService kleinserie,
    ScrapeMyrcmRaces myrcm)
{
    public async Task<List<RaceMeeting>> Retrieve()
    {
        var all = (await challenge.Parse()).ToList();

        var kleinserieAll = await kleinserie.Parse();
        all.AddRange(kleinserieAll);

        var dmcAll = await scrapeDmc.Parse();
        all.AddRange(dmcAll);
        
        var myrcmAll = await myrcm.Scrape();
        all.AddRange(myrcmAll);

        all.Sort((a, b) => a.Date.CompareTo(b.Date));

        return all;
    }
}