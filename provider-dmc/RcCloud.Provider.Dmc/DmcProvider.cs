using Microsoft.Extensions.DependencyInjection;
using RcCloud.Provider.Dmc.Calendar.Services;

namespace RcCloud.Provider.Dmc;

public static class DmcProvider
{
    public static IServiceCollection AddDmcProvider(this IServiceCollection services)
    {
        return services
            .AddTransient<GuessSeries>()
            .AddTransient<DownloadDmcCalendar>();
    }
}
