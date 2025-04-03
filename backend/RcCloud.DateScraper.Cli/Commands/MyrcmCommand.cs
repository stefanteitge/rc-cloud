using RcCloud.DateScraper.Application.Myrcm.Upcoming.Services;
using RcCloud.DateScraper.Cli.Output.Services;

namespace RcCloud.DateScraper.Cli.Commands;

internal class MyrcmCommand(ScrapeMyrcmRaces races, PrintRaces printer)
{
    public async Task OnExecute()
    {
        var all = await races.Scrape();
        printer.Print(all);
    }
}
