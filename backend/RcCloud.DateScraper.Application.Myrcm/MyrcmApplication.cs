using Microsoft.Extensions.DependencyInjection;
using RcCloud.DateScraper.Application.Myrcm.Services;

namespace RcCloud.DateScraper.Application.Myrcm;

public static class MyrcmApplication
{
    public static IServiceCollection AddMyrcm(this IServiceCollection services)
    {
        return services
            .AddTransient<DownloadMyrcmPageService>()
            .AddTransient<ParseMyrcmService>();
    }
}
