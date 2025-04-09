using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using RcCloud.DateScraper.Infrastructure.Races;

namespace RcCloud.FunctionApi.Functions;

public class Germany(MongoRaceRepository repository)
{
    [Function("germany")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        var races = await repository.Load("germany", "aggregate");

        return new OkObjectResult(races);
    }
}
