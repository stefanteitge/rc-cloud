namespace RcCloud.DateScraper.Application.Myrcm.Upcoming.Services;

public interface IDownloadMyrcmPages
{
    Task<string> Download(DownloadFilter filter, int pageIndex = 0);
}
