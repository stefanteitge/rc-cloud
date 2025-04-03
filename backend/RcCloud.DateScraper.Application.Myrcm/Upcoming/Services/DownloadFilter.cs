using RcCloud.DateScraper.Application.Myrcm.Common.Domain;

namespace RcCloud.DateScraper.Application.Myrcm.Upcoming.Services;

public class DownloadFilter
{
    public const string GermanyCookiePure = @"[{""field"":{""label"":""Country"",""value"":""Country""},""operator"":{""label"":""equals"",""value"":""eq""},""value"":{""label"":""\""Germany\"""",""value"":""3""}}]";

    private readonly MyrcmCountryCode _myrcmCountry;

    private DownloadFilter(MyrcmCountryCode myrcmCountry)
    {
        this._myrcmCountry = myrcmCountry;
    }

    public static DownloadFilter GermanyOnly => new(MyrcmCountryCode.Germany);

    public string GetCookie() => Base64Encode(GermanyCookiePure);

    private string Base64Encode(string plain)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(plain);
        return Convert.ToBase64String(bytes);
    }
}
