using RcCloud.DateScraper.Application.Myrcm.Common.Domain;
using RcCloud.DateScraper.Application.Myrcm.Upcoming.Services;

namespace RcCloud.DateScraper.Application.Myrcm.Tests.Upcoming.Services;

public class DownloadMyrcmPagesTests
{
    [Fact]
    public async Task Test1()
    {
        var sut = new DownloadMyrcmPages();
        var content = await sut.Download(new DownloadFilter([MyrcmCountryCode.Germany]));

        Assert.Contains("Vintage auf Intermodellbau", content);
        Assert.DoesNotContain("Avondcompetitie #1 HFCC Racing 2025", content);
    }

    [Fact]
    public async Task Parse_Page4_HasEvent()
    {
        var sut = new DownloadMyrcmPages();
        var content = await sut.Download(new DownloadFilter([MyrcmCountryCode.Germany]), 3);

        Assert.Contains("MCC-Herbst Open Freundschaftsrennen", content);
    }
}
