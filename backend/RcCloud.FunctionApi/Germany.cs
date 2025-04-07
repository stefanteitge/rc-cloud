using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using RcCloud.DateScraper.Domain.Races;
using RcCloud.DateScraper.Domain.Series;

namespace RcCloud.FunctionApi
{
    public class Germany
    {
        private readonly ILogger<Germany> _logger;

        public Germany(ILogger<Germany> logger)
        {
            _logger = logger;
        }

        [Function("germany")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult(new RaceMeeting([SeriesReference.None], SeasonReference.Current, new DateOnly(2025, 4, 7), "here", "de", "Foo race", [], null, null));
        }
    }
}
