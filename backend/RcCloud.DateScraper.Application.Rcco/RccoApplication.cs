using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using RcCloud.DateScraper.Application.Rcco.Services;
using RcCloud.DateScraper.Application.Rcco.Services.Impl;

[assembly: InternalsVisibleTo("RcCloud.DateScraper.Application.Rcco.Tests")]

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
