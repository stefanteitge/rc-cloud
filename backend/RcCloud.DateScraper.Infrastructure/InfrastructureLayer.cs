using Microsoft.Extensions.DependencyInjection;
using RcCloud.DateScraper.Domain.Clubs;
using RcCloud.DateScraper.Domain.Races;
using RcCloud.DateScraper.Infrastructure.Clubs.File;
using RcCloud.DateScraper.Infrastructure.Clubs.Mongo;
using RcCloud.DateScraper.Infrastructure.Races;

namespace RcCloud.DateScraper.Infrastructure;

public static class InfrastructureLayer
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        return services
            .AddScoped<IClubFileRepository, JsonClubFileRepository>()
            .AddScoped<IRaceCompilationRepository, MongoRaceCompilationRepository>()
            .AddScoped<IClubCopyRepository, MongoClubRepository>();
    }
}
