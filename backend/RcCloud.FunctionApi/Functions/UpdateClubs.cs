using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using RcCloud.DateScraper.Infrastructure.Clubs;
using RcCloud.DateScraper.Infrastructure.Clubs.Entities;
using RcCloud.DateScraper.Infrastructure.Clubs.Mongo;

namespace RcCloud.FunctionApi.Functions;

public class UpdateClubs(MongoClubRepository repo, ILogger<MongoClubRepository> logger)
{
    [Function("update-clubs")]
    public async Task<Results<BadRequest, Ok<ClubDbDocument>>> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        var client = new HttpClient();
        
        // TODO: actually retrieves ClubDbJsonRoot
        var db = await client.GetFromJsonAsync<ClubDbDocument>(
            "https://raw.githubusercontent.com/stefanteitge/rc-cloud/refs/heads/main/db/club-db.json");

        if (db is null)
        {
            return TypedResults.BadRequest();
        }

        // TODO remove this ugly fix while introducing DTOs 
        db.Compilation = "germany";
        
        //var db = new ClubDbEntity([], DateTimeOffset.Now);
        var success = await repo.Store(db);

        if (!success)
        {
            return TypedResults.BadRequest();
        }

        logger.LogInformation("Updated and stored clubs.");

        // TODO: use DTO
        return TypedResults.Ok(db);
    }
}
