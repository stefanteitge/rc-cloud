using Microsoft.Extensions.DependencyInjection;
using RcCloud.DateScraper.Domain.Clubs;
using RcCloud.DateScraper.Infrastructure.Clubs;

namespace RcCloud.DateScraper.Infrastructure;

public static class InfrastructureLayer
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        return services
            .AddScoped<IClubRepository, JsonClubRepository>()
            .AddScoped<MongoClubRepository>();
    }
}
