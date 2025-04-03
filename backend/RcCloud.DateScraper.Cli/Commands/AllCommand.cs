using RcCloud.DateScraper.Cli.Common.Services;
using RcCloud.DateScraper.Cli.Output.Services;

namespace RcCloud.DateScraper.Cli.Commands;

internal class AllCommand(
    RetrieveAllRaces retrieveAll,
    PrintRaces printer)
{
    public async Task OnExecute()
    {
        var all = await retrieveAll.Retrieve();

        printer.Print(all);
    }
}
