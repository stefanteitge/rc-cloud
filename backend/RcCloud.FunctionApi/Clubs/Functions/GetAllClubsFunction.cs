using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Azure.Functions.Worker;
using RcCloud.DateScraper.Infrastructure.Clubs.Mongo;
using RcCloud.FunctionApi.Clubs.Dto;

namespace RcCloud.FunctionApi.Clubs.Functions;

public class GetAllClubsFunction(MongoClubRepository repository)
{
    [Function("clubs")]
    public async Task<Results<Ok<List<ClubDto>>, NotFound>> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        var clubs = await repository.GetAll("germany");

        var dtos = clubs.Select(ClubDto.FromDomain).ToList();
        return TypedResults.Ok(dtos);
    }
}
