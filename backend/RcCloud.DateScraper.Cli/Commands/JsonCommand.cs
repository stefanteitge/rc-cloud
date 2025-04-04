using System.Text.Json;
using RcCloud.DateScraper.Cli.Common.Services;
using RcCloud.DateScraper.Cli.Output.Services;

namespace RcCloud.DateScraper.Cli.Commands;

internal class JsonCommand(RetrieveAllRaces retrieveAll, WriteJson writeJson)
{
    public async Task OnExecute()
    {
        var all = await retrieveAll.Retrieve();
        
        var options = new JsonSerializerOptions(JsonSerializerOptions.Web)
        {
            WriteIndented = true
        };

        writeJson.Write(all, "all.json");
    }
}
