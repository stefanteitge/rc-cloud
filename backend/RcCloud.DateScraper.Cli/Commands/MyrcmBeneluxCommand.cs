using McMaster.Extensions.CommandLineUtils;
using RcCloud.DateScraper.Application.Myrcm.Common.Domain;
using RcCloud.DateScraper.Application.Myrcm.Upcoming.Services;
using RcCloud.DateScraper.Cli.Output.Services;

namespace RcCloud.DateScraper.Cli.Commands;

internal class MyrcmBeneluxCommand(ScrapeMyrcmRaces races, PrintRaces printer, WriteJson writeJson)
{
    [Option("--format", CommandOptionType.SingleValue)]
    public string Format { get; set; } = "console";

    public async Task OnExecute()
    {
        MyrcmCountryCode[] beneluxFilter = [MyrcmCountryCode.Belgium, MyrcmCountryCode.Luxembourg, MyrcmCountryCode.Netherlands];
        var all = await races.Scrape(beneluxFilter);

        if (Format == "json")
        {
            writeJson.Write(all, "benelux.json");
            return;
        }

        printer.Print(all);
    }
}
