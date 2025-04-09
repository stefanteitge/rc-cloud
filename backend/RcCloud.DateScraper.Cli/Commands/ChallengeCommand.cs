using RcCloud.DateScraper.Application.Rck.Services;
using RcCloud.DateScraper.Cli.Output.Services;

namespace RcCloud.DateScraper.Cli.Commands;

internal class ChallengeCommand(ScrapeChallengeRaces races, PrintRaces printer)
{
    public async Task OnExecute()
    {
        var all = await races.Scrape();
        printer.Print(all);
    }
}
