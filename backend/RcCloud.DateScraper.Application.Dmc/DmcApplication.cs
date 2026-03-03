using Microsoft.Extensions.DependencyInjection;
using RcCloud.DateScraper.Application.Dmc.Calendar.Services;
using RcCloud.Provider.Dmc;

namespace RcCloud.DateScraper.Application.Dmc;

public static class DmcApplication
{
    public static IServiceCollection AddDmc(this IServiceCollection services)
    {
        return services
            .AddDmcProvider()
            .AddTransient<ScrapeDmcClubs>()
            .AddTransient<ScrapeDmcRaces>();
    }
}
