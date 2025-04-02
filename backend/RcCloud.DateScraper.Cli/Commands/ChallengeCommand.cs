using RcCloud.DateScraper.Application.Rck.Services;

namespace RcCloud.DateScraper.Cli.Commands;

internal class ChallengeCommand(ChallengeService service, RaceMeetingPrinter printer)
{
    public async Task OnExecute()
    {
        var all = await service.Parse();
        printer.Print(all);
    }
}
