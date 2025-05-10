using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Azure.Functions.Worker;
using RcCloud.DateScraper.Infrastructure.Races;
using RcCloud.FunctionApi.Races.Dto;

namespace RcCloud.FunctionApi.Races.Functions;

public class GetGermanyFunction(MongoRaceRepository repository)
{
    [Function("germany")]
    public async Task<Results<Ok<RacePageDto>, NotFound>> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        var racesDocument = await repository.Load("germany", "aggregate");

        if (racesDocument is null)
        {
            return TypedResults.NotFound();
        }

        var hasDmc = racesDocument.Races.Any(r => r.Source == "DMC");

        string? lastDmcUpdate = null;
        if (!hasDmc)
        {
            var dmcDocument = await repository.Load("germany", "dmc");

            if (dmcDocument is not null)
            {
                racesDocument.Races.AddRange(dmcDocument.Races);
                lastDmcUpdate = dmcDocument.LastUpdate;
            }
            
        }

        var dto = RacePageDto.FromDocument(racesDocument, RacePageDto.GermanyCategories, lastDmcUpdate);
        return TypedResults.Ok(dto);
    }
}
