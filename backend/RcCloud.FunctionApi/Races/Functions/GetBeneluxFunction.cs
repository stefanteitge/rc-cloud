using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Azure.Functions.Worker;
using RcCloud.DateScraper.Infrastructure.Races;
using RcCloud.FunctionApi.Races.Dto;

namespace RcCloud.FunctionApi.Races.Functions;

public class GetBeneluxFunction(MongoRaceRepository repository)
{
    [Function("benelux")]
    public async Task<Results<Ok<RacePageDto>, NotFound>> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        var racesDocument = await repository.Load("benelux", "aggregate");

        if (racesDocument is null)
        {
            return TypedResults.NotFound();
        }

        var today = DateOnly.FromDateTime(DateTime.Now);
        var races = racesDocument.Races.Where(r => r.Date >= today).ToList();
        
        var dto = RacePageDto.FromRaces(races, RacePageDto.BeneluxCategories, racesDocument.LastUpdate, null);
        return TypedResults.Ok(dto);
    }
}
