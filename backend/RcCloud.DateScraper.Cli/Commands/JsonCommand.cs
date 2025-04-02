using System.Text.Json;
using RcCloud.DateScraper.Cli.Commands.Utils;

namespace RcCloud.DateScraper.Cli.Commands;

internal class JsonCommand(RetrieveAllService retrieveAll)
{
    public async Task OnExecute()
    {
        var all = await retrieveAll.Retrieve();
        
        var options = new JsonSerializerOptions(JsonSerializerOptions.Web)
        {
            WriteIndented = true
        };
        var bytes = JsonSerializer.SerializeToUtf8Bytes(new JsonExportSchema(DateTimeOffset.Now, all), options);
        File.WriteAllBytes("all.json", bytes);
    }
}
