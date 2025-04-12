using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using RcCloud.DateScraper.Infrastructure.Clubs;
using RcCloud.DateScraper.Infrastructure.Clubs.Entities;

namespace RcCloud.FunctionApi.Functions;

public class UpdateClubs(MongoClubRepository repo, ILogger<MongoClubRepository> logger)
{
    [Function("update-clubs")]
    public async Task<Results<BadRequest, Ok<ClubDbEntity>>> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        var client = new HttpClient();
        var db = await client.GetFromJsonAsync<ClubDbEntity>(
            "https://raw.githubusercontent.com/stefanteitge/rc-cloud/refs/heads/main/db/club-db.json");

        if (db is null)
        {
            return TypedResults.BadRequest();
        }
        
        //var db = new ClubDbEntity([], DateTimeOffset.Now);
        var success = await repo.Store(db);

        if (!success)
        {
            return TypedResults.BadRequest();
        }

        logger.LogInformation("Updated and stored clubs.");

        return TypedResults.Ok(db);
    }
}
