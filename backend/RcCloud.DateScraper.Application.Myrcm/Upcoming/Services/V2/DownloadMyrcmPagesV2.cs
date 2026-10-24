namespace RcCloud.DateScraper.Application.Myrcm.Upcoming.Services.V2;

public class DownloadMyrcmPagesV2 : IDownloadMyrcmPages
{
    public const string UpcomingUrl = "https://www.myrcm.ch/de/upcoming";

    private static readonly HttpClient Client = new();

    /// <summary>
    /// Downloads one page of upcoming events as the JSON envelope the MyRCM pagination uses.
    /// </summary>
    public async Task<string> Download(DownloadFilter filter, int pageIndex = 0)
    {
        var form = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["cType"] = "json",
            ["ajax"] = "true",
        });

        var response = await Client.PostAsync(GetPageUrl(pageIndex), form);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    public static string GetPageUrl(int pageIndex) => $"{UpcomingUrl}?pId={pageIndex}";
}
