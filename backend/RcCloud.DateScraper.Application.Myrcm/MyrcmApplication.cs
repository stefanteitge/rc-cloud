using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using RcCloud.DateScraper.Application.Myrcm.Clubs.Services;
using RcCloud.DateScraper.Application.Myrcm.Upcoming.Services;
using RcCloud.DateScraper.Application.Myrcm.Upcoming.Services.Impl;

[assembly: InternalsVisibleTo("RcCloud.DateScraper.Application.Myrcm.Tests")]

namespace RcCloud.DateScraper.Application.Myrcm;

public static class MyrcmApplication
{
    public static IServiceCollection AddMyrcm(this IServiceCollection services)
    {
        return services
            .AddTransient<IEnhanceClub, EnhanceClub>()
            .AddTransient<GuessIfItIsTraining>()
            .AddTransient<DownloadMyrcmPages>()
            .AddTransient<GuessSeriesFromTitle>()
            .AddTransient<ScrapeMyrcmClubs>()
            .AddTransient<ScrapeMyrcmRaces>();
    }
}
