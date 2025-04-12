using Microsoft.Extensions.DependencyInjection;
using RcCloud.DateScraper.Domain.Clubs;
using RcCloud.DateScraper.Infrastructure.Clubs;
using RcCloud.DateScraper.Infrastructure.Clubs.Json;
using RcCloud.DateScraper.Infrastructure.Clubs.Mongo;
using RcCloud.DateScraper.Infrastructure.Races;

namespace RcCloud.DateScraper.Infrastructure;

public static class InfrastructureLayer
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        return services
            .AddScoped<IClubRepository, JsonClubRepository>()
            .AddScoped<MongoRaceRepository>()
            .AddScoped<MongoClubRepository>();
    }
}
