using McMaster.Extensions.CommandLineUtils;
using RcCloud.DateScraper.Cli.Common.Services;
using RcCloud.DateScraper.Cli.Output.Services;
using RcCloud.DateScraper.Domain.Clubs;

namespace RcCloud.DateScraper.Cli.Commands;

internal class GermanyCommand(
    IClubRepository clubRepository,
    RetrieveAllRaces retrieveAll,
    PrintRaces printer,
    WriteJson writeJson)
{
    [Option("--format", CommandOptionType.SingleValue)]
    public string Format { get; set; } = "console";
    
    [Option("--club-db", CommandOptionType.SingleValue)]
    [FileExists]
    public string ClubDbFile { get; set; } = "console";
    
    public async Task OnExecute()
    {
        if (!string.IsNullOrEmpty(ClubDbFile))
        {
            clubRepository.Load(ClubDbFile);
        }
        
        var all = await retrieveAll.Retrieve();

        if (Format == "json")
        {
            writeJson.Write(all, "germany.json");
            return;
        }

        printer.Print(all);
    }
}
