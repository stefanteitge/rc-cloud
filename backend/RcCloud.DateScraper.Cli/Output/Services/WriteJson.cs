using System.Text.Json;
using RcCloud.DateScraper.Cli.Output.Dto;

namespace RcCloud.DateScraper.Cli.Output.Services;
public class WriteJson
{
    public void Write<T>(T contents, string fileName)
    {
        var options = new JsonSerializerOptions(JsonSerializerOptions.Web)
        {
            WriteIndented = true
        };

        var bytes = JsonSerializer.SerializeToUtf8Bytes(new JsonExportSchema<T>(DateTimeOffset.Now, contents), options);
        File.WriteAllBytes(fileName, bytes);
    }
}
