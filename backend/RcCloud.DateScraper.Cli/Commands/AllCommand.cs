using RcCloud.DateScraper.Application.Dmc;
using RcCloud.DateScraper.Application.Rck.Services;

namespace RcCloud.DateScraper.Cli.Commands;

internal class AllCommand(
    ChallengeService challenge,
    DmcService dmc,
    KleinserieService kleinserie,
    RaceMeetingPrinter printer)
{
    public async Task OnExecute()
    {
        var all = (await challenge.Parse()).ToList();

        var kleinserieAll = await kleinserie.Parse();
        all.AddRange(kleinserieAll);

        var dmcAll = await dmc.Parse();
        all.AddRange(dmcAll);

        all.Sort((a, b) => a.Date.CompareTo(b.Date));

        printer.Print(all);
    }
}
