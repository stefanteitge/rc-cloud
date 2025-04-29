using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using RcCloud.DateScraper.Application.Dmc.Calendar.Services;
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

public class UpdateDmcFunction(
    ScrapeDmcRaces dmc,
    IClubRepository clubRepository,
    MongoClubRepository mongoClubRepository,
    MongoRaceRepository repo,
    ILogger<UpdateDmcFunction> logger)
{
    [Function("update-dmc")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        var clubs = await mongoClubRepository.GetAll("germany");
        clubRepository.Load(clubs);
        
        var all = new List<RaceMeeting>();
        
        var dmcResult = await dmc.Scrape();
        if (dmcResult.IsSuccess)
        {
            all.AddRange(dmcResult.Value);
            await repo.Store(dmcResult.Value, "germany", "dmc");
        }
        
        logger.LogInformation("Found {Count} races from DMC.", all.Count);

        return new OkObjectResult(GermanyPageDto.FromRaces(all, DateTimeOffset.Now.ToString()));
    }
}
