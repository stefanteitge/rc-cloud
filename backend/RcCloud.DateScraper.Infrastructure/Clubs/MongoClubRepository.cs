﻿using DnsClient.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using RcCloud.DateScraper.Infrastructure.Clubs.Entities;

namespace RcCloud.DateScraper.Infrastructure.Clubs;

public class MongoClubRepository(IConfiguration configuration, ILogger<MongoClubRepository> logger)
{
    public void Store(ClubDbEntity clubDbEntity)
    {
        var connectionUri = configuration.GetConnectionString("Mongo");

        if (string.IsNullOrEmpty(connectionUri))
        {
            logger.LogError("Mongo connection string is not set.");
            return;
        }

        var settings = MongoClientSettings.FromConnectionString(connectionUri);
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        var client = new MongoClient(settings);
        try
        {
            var collection = client
                .GetDatabase("RcCloud").GetCollection<ClubDbEntity>("Clubs"); ;
            collection.InsertOne(clubDbEntity);


            Console.WriteLine("Pinged your deployment. You successfully connected to MongoDB!");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}
