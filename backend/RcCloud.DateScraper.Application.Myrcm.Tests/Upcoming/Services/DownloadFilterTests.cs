using RcCloud.DateScraper.Application.Myrcm.Common.Domain;
using RcCloud.DateScraper.Application.Myrcm.Upcoming.Services;

namespace RcCloud.DateScraper.Application.Myrcm.Tests.Upcoming.Services;

public class DownloadFilterTests
{
    [Fact]
    public void GetCookie_WithGermany_CreatesCorrectBase64()
    {
        var filter = new DownloadFilter([MyrcmCountryCode.Germany]);

        var cookie = filter.GetCookie();

        Assert.Equal("W3siZmllbGQiOnsibGFiZWwiOiJDb3VudHJ5IiwidmFsdWUiOiJDb3VudHJ5In0sIm9wZXJhdG9yIjp7ImxhYmVsIjoiZXF1YWxzIiwidmFsdWUiOiJlcSJ9LCJ2YWx1ZSI6eyJsYWJlbCI6IlwiR2VybWFueVwiIiwidmFsdWUiOiIzIn19XQ==", cookie);
    }

    [Fact]
    public void GetCookie_WithBeneluxModel_CreatesCorrectBase64()
    {
        var filter = new DownloadFilter([MyrcmCountryCode.Belgium, MyrcmCountryCode.Luxembourg, MyrcmCountryCode.Netherlands]);

        var cookie = filter.GetCookie();

        Assert.Equal("W3siZmllbGQiOnsibGFiZWwiOiJDb3VudHJ5IiwidmFsdWUiOiJDb3VudHJ5In0sIm9wZXJhdG9yIjp7ImxhYmVsIjoiYW55IG9mIiwidmFsdWUiOiJpbiJ9LCJ2YWx1ZSI6eyJsYWJlbCI6IihCZWxnaXVtLCBMdXhlbWJvdXJnLCBOZXRoZXJsYW5kcykiLCJ2YWx1ZSI6IjgsNSw5In19XQ==", cookie);
    }
}
