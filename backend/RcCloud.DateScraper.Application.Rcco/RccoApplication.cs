using Microsoft.Extensions.DependencyInjection;
using RcCloud.DateScraper.Application.Rcco.Services;
using RcCloud.DateScraper.Application.Rcco.Services.Impl;

namespace RcCloud.DateScraper.Application.Rcco;

public static class RccoApplication
{
    public static IServiceCollection AddRcco(this IServiceCollection services)
    {
        return services
            .AddTransient<ScrapeRcco>()
            .AddTransient<IGuessClub, GuessClub>();
    }
}
