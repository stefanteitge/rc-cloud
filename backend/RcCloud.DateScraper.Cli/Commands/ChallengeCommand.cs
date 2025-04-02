﻿using RcCloud.DateScraper.Application.Rck.Services;
using RcCloud.DateScraper.Cli.Commands.Utils;

namespace RcCloud.DateScraper.Cli.Commands;

internal class ChallengeCommand(ChallengeService service, RaceMeetingPrinter printer)
{
    public async Task OnExecute()
    {
        var all = await service.Parse();
        printer.Print(all);
    }
}
