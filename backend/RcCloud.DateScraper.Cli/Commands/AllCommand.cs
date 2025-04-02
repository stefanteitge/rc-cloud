using RcCloud.DateScraper.Cli.Commands.Utils;

namespace RcCloud.DateScraper.Cli.Commands;

internal class AllCommand(
    RetrieveAllService retrieveAll,
    RaceMeetingPrinter printer)
{
    public async Task OnExecute()
    {
        var all = await retrieveAll.Retrieve();

        printer.Print(all);
    }
}
