using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using RcCloud.DateScraper.Application.Dmc.Calendar.Services;
using RcCloud.DateScraper.Application.Myrcm.Common.Domain;
using RcCloud.DateScraper.Application.Myrcm.Upcoming.Services;
using RcCloud.DateScraper.Application.Rcco;
using RcCloud.DateScraper.Application.Rck.Services;
using RcCloud.DateScraper.Domain.Races;
using RcCloud.DateScraper.Infrastructure.Races;

namespace RcCloud.FunctionApi.Functions;

public class UpdateGermany(
    ScrapeChallengeRaces challenge,
    ScrapeDmcRaces dmc,
    ScrapeKleinserieRaces kleinserie,
    ScrapeMyrcmRaces myrcm,
    ScrapeRcco rcco,
    MongoRaceRepository repo,
    ILogger<UpdateGermany> logger)
{
    [Function("update-germany")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        var all = new List<RaceMeeting>();

        var challengeAll = await challenge.Scrape();
        all.AddRange(challengeAll);
        await repo.Store(challengeAll, "germany", "challenge");

        var kleinserieAll = await kleinserie.Scrape();
        all.AddRange(kleinserieAll);
        await repo.Store(kleinserieAll, "germany", "kleinserie");

        var dmcResult = await dmc.Scrape();
        if (dmcResult.IsSuccess)
        {
            all.AddRange(dmcResult.Value);
            await repo.Store(dmcResult.Value, "germany", "dmc");
        }

        var myrcmAll = await myrcm.Scrape([MyrcmCountryCode.Germany]);
        all.AddRange(myrcmAll);
        await repo.Store(myrcmAll, "germany", "myrcm");

        var rccoAll = await rcco.Scrape();
        all.AddRange(rccoAll);
        await repo.Store(rccoAll, "germany", "rcco");

        all.Sort((a, b) => a.Date.CompareTo(b.Date));
        await repo.Store(all, "germany", "aggregate");

        logger.LogInformation("Found {Count} races from all sources.", all.Count);

        return new OkObjectResult(all);
    }
}
