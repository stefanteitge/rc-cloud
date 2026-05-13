using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using RcCloud.DateScraper.Domain.Clubs;
using RcCloud.DateScraper.Domain.Races;
using RcCloud.DateScraper.Infrastructure.Clubs.File;
using RcCloud.DateScraper.Infrastructure.Clubs.Mongo;
using RcCloud.DateScraper.Infrastructure.Common.Configuration;
using RcCloud.DateScraper.Infrastructure.Races;

namespace RcCloud.DateScraper.Infrastructure;

public static class InfrastructureLayer
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // MongoDB
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDb").Bind);
        var mongoSettings = configuration.GetSection("MongoDb").Get<MongoDbSettings>()
                            ?? new MongoDbSettings { ConnectionString = "mongodb://localhost:27017", DatabaseName = "rccloud_api" };

        var mongoClientSettings = MongoClientSettings.FromConnectionString(mongoSettings.ConnectionString);
        if (!string.IsNullOrWhiteSpace(mongoSettings.Username) && !string.IsNullOrWhiteSpace(mongoSettings.Password))
        {
            mongoClientSettings.Credential = MongoCredential.CreateCredential(
                databaseName: mongoSettings.AuthSource,
                username: mongoSettings.Username,
                password: mongoSettings.Password);
        }

        services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoClientSettings));
        services.AddSingleton(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(mongoSettings.DatabaseName));
        
        return services
            .AddScoped<IClubFileRepository, JsonClubFileRepository>()
            .AddScoped<IRaceCompilationRepository, MongoRaceCompilationRepository>()
            .AddScoped<IClubCopyRepository, MongoClubRepository>();
    }
}
