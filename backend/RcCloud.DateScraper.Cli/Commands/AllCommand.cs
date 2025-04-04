using McMaster.Extensions.CommandLineUtils;
using RcCloud.DateScraper.Cli.Common.Services;
using RcCloud.DateScraper.Cli.Output.Services;

namespace RcCloud.DateScraper.Cli.Commands;

internal class AllCommand(
    RetrieveAllRaces retrieveAll,
    PrintRaces printer,
    WriteJson writeJson)
{
    [Option("--format", CommandOptionType.SingleValue)]
    public string Format { get; set; } = "console";
    
    public async Task OnExecute()
    {
        var all = await retrieveAll.Retrieve();

        if (Format == "json")
        {
            writeJson.Write(all, "all.json");
            return;
        }

        printer.Print(all);
    }
}
