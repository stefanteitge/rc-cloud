using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Azure.Functions.Worker;
using RcCloud.DateScraper.Infrastructure.Races;
using RcCloud.FunctionApi.Races.Dto;

namespace RcCloud.FunctionApi.Races.Functions;

public class GetGermanyFunction(MongoRaceRepository repository)
{
    [Function("germany")]
    public async Task<Results<Ok<GermanyPageDto>, NotFound>> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        var racesDocument = await repository.Load("germany", "aggregate");

        if (racesDocument is null)
        {
            return TypedResults.NotFound();
        }

        var dto = GermanyPageDto.FromDocument(racesDocument);
        return TypedResults.Ok(dto);
    }
}
