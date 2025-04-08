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
    public Results<BadRequest, Ok<ClubDbEntity>> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        var db = new ClubDbEntity([], DateTimeOffset.Now);
        var success = repo.Store(db);

        if (!success)
        {
            return TypedResults.BadRequest();
        }

        logger.LogInformation("Updated and stored clubs.");

        return TypedResults.Ok(db);
    }
}
