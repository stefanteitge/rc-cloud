using RcCloud.DateScraper.Application.Dmc;
using RcCloud.DateScraper.Application.Dmc.Calendar.Services;
using RcCloud.DateScraper.Cli.Output.Services;

namespace RcCloud.DateScraper.Cli.Commands;

internal class DmcCommand(ScrapeDmcRaces races, PrintRaces printer)
{
    public async Task OnExecute()
    {
        var all = await races.Parse();
        printer.Print(all);
    }
}
