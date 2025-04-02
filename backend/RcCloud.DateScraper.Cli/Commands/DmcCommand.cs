﻿using RcCloud.DateScraper.Application.Dmc;
using RcCloud.DateScraper.Cli.Commands.Utils;

namespace RcCloud.DateScraper.Cli.Commands;

internal class DmcCommand(DmcService service, RaceMeetingPrinter printer)
{
    public async Task OnExecute()
    {
        var all = await service.Parse();
        printer.Print(all);
    }
}
