using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Azure.Functions.Worker;
using RcCloud.DateScraper.Infrastructure.Clubs.Mongo;
using RcCloud.FunctionApi.Functions.Dto;

namespace RcCloud.FunctionApi.Functions;

public class Clubs(MongoClubRepository repository)
{
    [Function("clubs")]
    public async Task<Results<Ok<ClubDbDocument>, NotFound>> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        var racesDocument = await repository.GetAll("germany");

        if (racesDocument is null)
        {
            return TypedResults.NotFound();
        }
        
        return TypedResults.Ok(racesDocument);
    }
}
