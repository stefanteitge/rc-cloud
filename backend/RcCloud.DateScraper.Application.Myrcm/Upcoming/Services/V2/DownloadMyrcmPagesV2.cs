namespace RcCloud.DateScraper.Application.Myrcm.Upcoming.Services.V2;

public class DownloadMyrcmPagesV2 : IDownloadMyrcmPages
{
    public const string UpcomingUrl = "https://www.myrcm.ch/de/upcoming";

    private static readonly HttpClient Client = new();

    public Task<string> Download(DownloadFilter filter, int pageIndex = 0)
    {
        return Client.GetStringAsync(UpcomingUrl);
    }
}
