using System.Net;

namespace RcCloud.DateScraper.Application.Myrcm.Services;

public class DownloadMyrcmPageService
{
    public const string BaseAddress = "https://www.myrcm.ch";

    public const string Page1Url = "https://www.myrcm.ch/myrcm/main?hId[1]=com&pLa=en";

    public const string PageNUrl = "https://www.myrcm.ch/myrcm/main?pLa=en&pId[E]=PPP&hId[1]=com";

    public const string CookieName = "MYRCM_EVENT_FILTER";

    public async Task<string> Download(DownloadFilter filter, int pageIndex = 0)
    {
        var baseUrl = Page1Url;

        if (pageIndex > 0)
        {
            baseUrl = PageNUrl.Replace("PPP", pageIndex.ToString());
        }

        var cookieContainer = new CookieContainer();
        var handler = new HttpClientHandler() { CookieContainer = cookieContainer };

        var client = new HttpClient(handler);

        cookieContainer.Add(new Uri(BaseAddress), new Cookie(CookieName, filter.GetCookie()));
        var response = await client.PostAsync(baseUrl, null);

        return await response.Content.ReadAsStringAsync();
    }
}
