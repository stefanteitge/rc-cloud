using Microsoft.Extensions.DependencyInjection;
using RcCloud.DateScraper.Application.Myrcm.Clubs.Services;
using RcCloud.DateScraper.Application.Myrcm.Upcoming.Services;

namespace RcCloud.DateScraper.Application.Myrcm;

public static class MyrcmApplication
{
    public static IServiceCollection AddMyrcm(this IServiceCollection services)
    {
        return services
            .AddTransient<GuessIfItIsTraining>()
            .AddTransient<SanitizeClubNames>()
            .AddTransient<DownloadMyrcmPages>()
            .AddTransient<GuessSeriesFromTitle>()
            .AddTransient<ScrapeMyrcmClubs>()
            .AddTransient<ScrapeMyrcmRaces>();
    }
}
