using System.Net;

namespace RcCloud.DateScraper.Application.Myrcm.Services;

public class MyrcmService
{
    public const string BaseAddress = "https://www.myrcm.ch";

    public const string Page1Url = "https://www.myrcm.ch/myrcm/main?hId[1]=com&pLa=en";

    public const string PageNUrl = "https://www.myrcm.ch/myrcm/main?pLa=en&pId[E]=PPP&hId[1]=com";

    public const string CookieName = "MYRCM_EVENT_FILTER";

    public const string GermanyCookie = "W3siZmllbGQiOnsibGFiZWwiOiJDb3VudHJ5IiwidmFsdWUiOiJDb3VudHJ5In0sIm9wZXJhdG9yIjp7ImxhYmVsIjoiZXF1YWxzIiwidmFsdWUiOiJlcSJ9LCJ2YWx1ZSI6eyJsYWJlbCI6IlwiR2VybWFueVwiIiwidmFsdWUiOiIzIn19XQ==";

    public const string GermanyCookiePure = @"[{""field"":{""label"":""Country"",""value"":""Country""},""operator"":{""label"":""equals"",""value"":""eq""},""value"":{""label"":""\""Germany\"""",""value"":""3""}}]";

    public async Task<string> Parse(int pageIndex = 0)
    {
        var baseUrl = Page1Url;

        if (pageIndex > 0)
        {
            baseUrl = PageNUrl.Replace("PPP", pageIndex.ToString());
        }

        var cookieContainer = new CookieContainer();
        var handler = new HttpClientHandler() { CookieContainer = cookieContainer };

        var client = new HttpClient(handler);

        cookieContainer.Add(new Uri(BaseAddress), new Cookie(CookieName, Base64Encode(GermanyCookiePure)));
        var response = await client.PostAsync(baseUrl, null);

        return await response.Content.ReadAsStringAsync();
    }

    public string Base64Encode(string plain)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(plain);
        return Convert.ToBase64String(bytes);
    }
}
