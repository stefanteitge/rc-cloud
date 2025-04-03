using RcCloud.DateScraper.Application.Rck.Services;
using RcCloud.DateScraper.Cli.Output.Services;

namespace RcCloud.DateScraper.Cli.Commands;

internal class KleinserieCommand(ScrapeKleinserieRaces races, PrintRaces printer)
{
    public async Task OnExecute()
    {
        var all = await races.Parse();
        printer.Print(all);
    }
}
