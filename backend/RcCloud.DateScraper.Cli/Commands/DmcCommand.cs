using RcCloud.DateScraper.Application.Dmc;
using RcCloud.DateScraper.Cli.Commands.Utils;

namespace RcCloud.DateScraper.Cli.Commands;

internal class DmcCommand(ScrapeDmcRaces races, RaceMeetingPrinter printer)
{
    public async Task OnExecute()
    {
        var all = await races.Parse();
        printer.Print(all);
    }
}
