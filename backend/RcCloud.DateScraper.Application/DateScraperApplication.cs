using Microsoft.Extensions.DependencyInjection;
using RcCloud.DateScraper.Application.Common.Services;
using RcCloud.DateScraper.Application.Dmc;
using RcCloud.DateScraper.Application.Myrcm;
using RcCloud.DateScraper.Application.Rcco;
using RcCloud.DateScraper.Application.Rck;

namespace RcCloud.DateScraper.Application.Common;

public static class DateScraperApplication
{
    public static IServiceCollection AddScraping(this IServiceCollection services)
    {
        return services
            .AddDmc()
            .AddMyrcm()
            .AddRcco()
            .AddRck()
            .AddTransient<RetrieveAllGermanRaces>();
    }
}
