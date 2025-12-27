using McMaster.Extensions.CommandLineUtils;
using RcCloud.DateScraper.Application.Myrcm.Common.Domain;
using RcCloud.DateScraper.Application.Myrcm.Upcoming.Services;
using RcCloud.DateScraper.Cli.Output.Services;

namespace RcCloud.DateScraper.Cli.Commands;

[Command(name: "myrcm-germany")]
internal class MyrcmGermanyCommand(ScrapeMyrcmRaces races, PrintRaces printer)
{
    public async Task OnExecute()
    {
        var all = await races.Scrape([MyrcmCountryCode.Germany]);
        printer.Print(all);
    }
}
