using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using RcCloud.DateScraper.Application.Common.Services;
using RcCloud.DateScraper.Domain.Races;
using RcCloud.DateScraper.Domain.Series;
using RcCloud.DateScraper.Infrastructure.Races;

namespace RcCloud.FunctionApi.Functions;

public class Germany(RetrieveAllGermanRaces retrieveAllGermanRaces, MongoRaceRepository repo, ILogger<Germany> logger)
{
    [Function("germany")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        logger.LogInformation("C# HTTP trigger function processed a request.");
        var all = await retrieveAllGermanRaces.Retrieve();
        
        await repo.Store(all);
        
        return new OkObjectResult(all);
    }
}
