using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using RcCloud.DateScraper.Infrastructure.Clubs;
using RcCloud.DateScraper.Infrastructure.Clubs.Entities;

namespace RcCloud.FunctionApi.Functions;

public class UpdateClubs(MongoClubRepository repo, ILogger<MongoClubRepository> logger)
{
    [Function("update-clubs")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        var db = new ClubDbEntity([], DateTimeOffset.Now);
        repo.Store(db);

        logger.LogInformation("Updated and stored clubs.");

        return new OkObjectResult(db);
    }
}
