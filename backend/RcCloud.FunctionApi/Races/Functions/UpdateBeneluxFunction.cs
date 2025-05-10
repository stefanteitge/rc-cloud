using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using RcCloud.DateScraper.Application.Myrcm.Common.Domain;
using RcCloud.DateScraper.Application.Myrcm.Upcoming.Services;
using RcCloud.DateScraper.Application.Rcco;
using RcCloud.DateScraper.Application.Rck.Services;
using RcCloud.DateScraper.Domain.Clubs;
using RcCloud.DateScraper.Domain.Races;
using RcCloud.DateScraper.Infrastructure.Clubs.Mongo;
using RcCloud.DateScraper.Infrastructure.Races;
using RcCloud.FunctionApi.Races.Dto;

namespace RcCloud.FunctionApi.Races.Functions;

public class UpdateBeneluxFunction(
    ScrapeMyrcmRaces myrcm,
    IClubRepository clubRepository,
    MongoClubRepository mongoClubRepository,
    MongoRaceRepository repo,
    ILogger<UpdateBeneluxFunction> logger)
{
    [Function("update-benelux")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        var clubs = await mongoClubRepository.GetAll("germany");
        clubRepository.Load(clubs);
        
        var all = new List<RaceMeeting>();
        
        var myrcmAll = await myrcm.Scrape([MyrcmCountryCode.Belgium, MyrcmCountryCode.Luxembourg, MyrcmCountryCode.Netherlands]);
        all.AddRange(myrcmAll);
        // await repo.Store(myrcmAll, "benelux", "myrcm");
        
        all.Sort((a, b) => a.Date.CompareTo(b.Date));
        await repo.Store(all, "benelux", "aggregate");

        logger.LogInformation("Found {Count} races in BeNeLux from all sources.", all.Count);

        return new OkObjectResult(RacePageDto.FromRaces(all, DateTimeOffset.Now.ToString(), null));
    }
}
