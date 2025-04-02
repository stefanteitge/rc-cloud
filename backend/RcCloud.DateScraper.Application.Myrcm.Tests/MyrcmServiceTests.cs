using RcCloud.DateScraper.Application.Myrcm.Services;

namespace RcCloud.DateScraper.Application.Myrcm.Tests;

public class MyrcmServiceTests
{
    [Fact]
    public async Task Test1()
    {
        var sut = new MyrcmService();
        var content = await sut.Parse();

        Assert.Contains("Vintage auf Intermodellbau", content);
        Assert.DoesNotContain("Avondcompetitie #1 HFCC Racing 2025", content);
    }

    [Fact]
    public async Task Parse_Page4_HasEvent()
    {
        var sut = new MyrcmService();
        var content = await sut.Parse(3);

        Assert.Contains("MCC-Herbst Open Freundschaftsrennen", content);
    }
}
