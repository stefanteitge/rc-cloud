using McMaster.Extensions.CommandLineUtils;
using RcCloud.DateScraper.Application.Dmc.Calendar.Services;
using RcCloud.DateScraper.Cli.Output;
using RcCloud.DateScraper.Cli.Output.Services;
using RcCloud.DateScraper.Domain.Clubs;

namespace RcCloud.DateScraper.Cli.Commands;

internal class DmcCommand(ScrapeDmcRaces races, PrintRaces printer, IClubRepository clubRepository)
{
    [Option("--club-db", CommandOptionType.SingleValue)]
    [FileExists]
    public string ClubDbFile { get; set; } = "console";
    
    public async Task OnExecute()
    {
        if (!string.IsNullOrEmpty(ClubDbFile))
        {
            clubRepository.Load(ClubDbFile);
        }
        
        var parseResult = await races.Scrape();

        if (parseResult.IsFailed)
        {
            throw new FluentException("DMC scraping failed", parseResult.ToResult());
        }

        printer.Print(parseResult.Value);
    }
}
