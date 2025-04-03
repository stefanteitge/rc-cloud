using Microsoft.Extensions.DependencyInjection;
using RcCloud.DateScraper.Application.Rck.Services;

namespace RcCloud.DateScraper.Application.Rck;

public static class RckApplication
{
    public static IServiceCollection AddRck(this IServiceCollection services)
    {
        return services
            .AddTransient<ScrapeChallengeRaces>()
            .AddTransient<ScrapeKleinserieRaces>();
    }
}
