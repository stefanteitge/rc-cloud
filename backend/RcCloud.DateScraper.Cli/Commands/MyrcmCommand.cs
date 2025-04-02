using RcCloud.DateScraper.Application.Myrcm.Services;
using RcCloud.DateScraper.Cli.Commands.Utils;

namespace RcCloud.DateScraper.Cli.Commands;

internal class MyrcmCommand(ScrapeMyrcmRaces races, RaceMeetingPrinter printer)
{
    public async Task OnExecute()
    {
        var all = await races.Scrape();
        printer.Print(all);
    }
}
