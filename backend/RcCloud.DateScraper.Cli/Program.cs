using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RcCloud.DateScraper.Application.Rck;
using RcCloud.DateScraper.Cli.Commands;
using RcCloud.DateScraper.Cli.Kleinserie;

await new HostBuilder()
    .ConfigureLogging((context, builder) =>
    {
        //builder.AddConsole();
    })
    .ConfigureServices((context, services) =>
    {
        services
            .AddSingleton<RaceMeetingPrinter>()
            .AddRck()
            .AddSingleton<IConsole>(PhysicalConsole.Singleton);
    })
    .RunCommandLineApplicationAsync<RootCommand>(args);


