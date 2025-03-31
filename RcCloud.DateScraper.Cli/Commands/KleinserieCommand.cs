using RcCloud.DateScraper.Rck.Application.Services;

namespace RcCloud.DateScraper.Cli.Commands;

internal class KleinserieCommand(KleinserieService service, RaceMeetingPrinter printer)
{
    public async Task OnExecute()
    {
        var all = await service.Parse();
        printer.Print(all);
    }
}
