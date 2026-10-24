using System.Text.Json;

namespace RcCloud.DateScraper.Application.Myrcm.Upcoming.Services.V2;

/// <summary>
/// One page of the MyRCM upcoming list, as returned by <c>POST /de/upcoming?pId=N</c> with <c>cType=json</c>.
/// The envelope looks like <c>{"STATUS":"OK","DATA":["&lt;html&gt;"],"RECORDCOUNT":[469],"FROMINDEX":[1],"TOINDEX":[25]}</c>.
/// </summary>
public record MyrcmUpcomingPage(string Html, int RecordCount, int FromIndex, int ToIndex)
{
    public bool IsLastPage => ToIndex >= RecordCount;

    public static MyrcmUpcomingPage Parse(string json)
    {
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        return new MyrcmUpcomingPage(
            GetFirst(root, "DATA").GetString() ?? string.Empty,
            GetFirst(root, "RECORDCOUNT").GetInt32(),
            GetFirst(root, "FROMINDEX").GetInt32(),
            GetFirst(root, "TOINDEX").GetInt32());
    }

    private static JsonElement GetFirst(JsonElement root, string name)
    {
        var element = root.GetProperty(name);
        return element.ValueKind == JsonValueKind.Array ? element[0] : element;
    }
}
