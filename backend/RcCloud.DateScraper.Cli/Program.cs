﻿using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RcCloud.DateScraper.Application.Dmc;
using RcCloud.DateScraper.Application.Myrcm;
using RcCloud.DateScraper.Application.Rck;
using RcCloud.DateScraper.Cli.Commands;
using RcCloud.DateScraper.Cli.Common.Services;
using RcCloud.DateScraper.Cli.Output.Services;

await new HostBuilder()
    .ConfigureLogging((context, builder) =>
    {
        //builder.AddConsole();
    })
    .ConfigureServices((context, services) =>
    {
        services
            .AddSingleton<PrintRaces>()
            .AddTransient<RetrieveAllRaces>()
            .AddDmc()
            .AddMyrcm()
            .AddRck()
            .AddSingleton<IConsole>(PhysicalConsole.Singleton);
    })
    .RunCommandLineApplicationAsync<RootCommand>(args);


