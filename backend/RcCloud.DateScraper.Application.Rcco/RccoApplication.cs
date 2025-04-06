using Microsoft.Extensions.DependencyInjection;

namespace RcCloud.DateScraper.Application.Rcco;

public static class RccoApplication
{
    public static IServiceCollection AddRcco(this IServiceCollection services)
    {
        return services
            .AddTransient<ScrapeRcco>();
    }
}
