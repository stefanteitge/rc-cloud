using Microsoft.Extensions.DependencyInjection;
using RcCloud.DateScraper.Rck.Application.Services;

namespace RcCloud.DateScraper.Rck.Application
{
    public static class RckApplication
    {
        public static IServiceCollection AddRck(this IServiceCollection services)
        {
            return services
                .AddTransient<ChallengeService>()
                .AddTransient<KleinserieService>();
        }
    }
}
