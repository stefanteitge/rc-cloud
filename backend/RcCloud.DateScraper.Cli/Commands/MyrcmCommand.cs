using RcCloud.DateScraper.Application.Myrcm.Services;
using RcCloud.DateScraper.Cli.Commands.Utils;

namespace RcCloud.DateScraper.Cli.Commands;

internal class MyrcmCommand(ParseMyrcmService service, RaceMeetingPrinter printer)
{
    public async Task OnExecute()
    {
        var all = await service.Parse();
        printer.Print(all);
    }
}
