using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
    public async Task<Results<Ok<RacePageDto>, BadRequest<string>>> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        var clubs = await mongoClubRepository.GetAll("germany");
        clubRepository.Load(clubs);
        
        var dmcResult = await dmc.Scrape(2025);
        if (dmcResult.IsFailed)
        {
            return TypedResults.BadRequest(dmcResult.Errors.FirstOrDefault()?.Message ?? "Scraping DMC failed.");
        }
        
        await repo.Store(dmcResult.Value, "germany", "dmc");
        logger.LogInformation("Found {Count} races from DMC.", dmcResult.Value.Count());

        return TypedResults.Ok(RacePageDto.FromRaces(dmcResult.Value, RacePageDto.GermanyCategories, DateTimeOffset.Now.ToString(), DateTimeOffset.Now.ToString()));
    }
}
