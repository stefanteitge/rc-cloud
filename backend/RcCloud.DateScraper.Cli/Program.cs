using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RcCloud.DateScraper.Application.Common;
using RcCloud.DateScraper.Application.Common.Services;
using RcCloud.DateScraper.Application.Dmc;
using RcCloud.DateScraper.Application.Myrcm;
using RcCloud.DateScraper.Application.Rcco;
using RcCloud.DateScraper.Application.Rck;
using RcCloud.DateScraper.Cli.Commands;
using RcCloud.DateScraper.Cli.Output.Services;
using RcCloud.DateScraper.Infrastructure;

await new HostBuilder()
    .ConfigureLogging((context, builder) =>
    {
        //builder.AddConsole();
    })
    .ConfigureServices((context, services) =>
    {
        services
            .AddInfrastructure()
            .AddSingleton<PrintRaces>()
            .AddTransient<WriteJson>()
            .AddScraping()
            .AddSingleton<IConsole>(PhysicalConsole.Singleton);
    })
    .RunCommandLineApplicationAsync<RootCommand>(args);


