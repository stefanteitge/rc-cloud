﻿using Microsoft.Extensions.DependencyInjection;
using RcCloud.DateScraper.Application.Dmc.Services;

namespace RcCloud.DateScraper.Application.Dmc;

public static class DmcApplication
{
    public static IServiceCollection AddDmc(this IServiceCollection services)
    {
        return services
            .AddTransient<ScrapeDmcRaces>()
            .AddTransient<DownloadDmcCalendarService>();
    }
}
