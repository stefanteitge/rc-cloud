using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using RcCloud.DateScraper.Infrastructure.Clubs.Json;
using RcCloud.DateScraper.Infrastructure.Clubs.Mongo;
using RcCloud.FunctionApi.Clubs.Dto;

namespace RcCloud.FunctionApi.Clubs.Functions;

public class UpdateClubsFunction(MongoClubRepository repo, ILogger<MongoClubRepository> logger)
{
    [Function("update-clubs")]
    public async Task<Results<BadRequest, Ok<List<ClubDto>>>> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        var client = new HttpClient();
        
        var db = await client.GetFromJsonAsync<ClubDbJsonRoot>(
            "https://raw.githubusercontent.com/stefanteitge/rc-cloud/refs/heads/main/db/club-db.json");

        if (db is null)
        {
            return TypedResults.BadRequest();
        }
        
        var clubs = db.Clubs.Select(c => c.ToDomain()).ToList();
        var success = await repo.Store(clubs);

        if (!success)
        {
            return TypedResults.BadRequest();
        }

        logger.LogInformation("Updated and stored clubs.");

        var dtos = clubs.Select(ClubDto.FromDomain).ToList();
        return TypedResults.Ok(dtos);
    }
}
