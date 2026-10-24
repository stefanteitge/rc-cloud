using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using RcCloud.DateScraper.Application.Myrcm.Clubs.Services;
using RcCloud.DateScraper.Application.Myrcm.Upcoming.Services;
using RcCloud.DateScraper.Application.Myrcm.Upcoming.Services.V2;
using RcCloud.DateScraper.Application.Myrcm.Upcoming.Services.Impl;

[assembly: InternalsVisibleTo("RcCloud.DateScraper.Application.Myrcm.Tests")]

namespace RcCloud.DateScraper.Application.Myrcm;

public static class MyrcmApplication
{
    public static IServiceCollection AddMyrcm(this IServiceCollection services)
    {
        return services
            .AddTransient<IGuessClub, GuessClub>()
            .AddTransient<GuessIfItIsTraining>()
            .AddTransient<IGuessIfItIsTraining, GuessIfItIsTraining>()
            .AddTransient<DownloadMyrcmPages>()
            .AddTransient<IDownloadMyrcmPages, DownloadMyrcmPages>()
            .AddTransient<GuessSeriesFromTitle>()
            .AddTransient<IGuessSeriesFromTitle, GuessSeriesFromTitle>()
            .AddTransient<IScrapeMyrcmClubs, ScrapeMyrcmClubs>()
            .AddTransient<IScrapeMyrcmRaces, ScrapeMyrcmRaces>()
            .AddTransient<ScrapeMyrcmClubs>()
            .AddTransient<ScrapeMyrcmRaces>();
    }

    public static IServiceCollection AddMyrcmV2(this IServiceCollection services)
    {
        return services
            .AddMyrcm()
            .AddTransient<IDownloadMyrcmPages, DownloadMyrcmPagesV2>()
            .AddTransient<IScrapeMyrcmRaces, ScrapeMyrcmRacesV2>();
    }
}
