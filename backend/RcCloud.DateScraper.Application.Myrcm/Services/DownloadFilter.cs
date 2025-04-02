namespace RcCloud.DateScraper.Application.Myrcm.Services;

public class DownloadFilter
{
    public const string GermanyCookiePure = @"[{""field"":{""label"":""Country"",""value"":""Country""},""operator"":{""label"":""equals"",""value"":""eq""},""value"":{""label"":""\""Germany\"""",""value"":""3""}}]";

    private readonly CountryCode country;

    private DownloadFilter(CountryCode country)
    {
        this.country = country;
    }

    public static DownloadFilter GermanyOnly => new(CountryCode.Germany);

    public string GetCookie() => Base64Encode(GermanyCookiePure);

    private string Base64Encode(string plain)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(plain);
        return Convert.ToBase64String(bytes);
    }
}
