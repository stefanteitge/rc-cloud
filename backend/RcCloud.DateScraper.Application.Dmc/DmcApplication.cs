using Microsoft.Extensions.DependencyInjection;

namespace RcCloud.DateScraper.Application.Dmc;

public static class DmcApplication
{
    public static IServiceCollection AddDmc(this IServiceCollection services)
    {
        return services
            .AddTransient<DmcService>();
    }
}
