using RcCloud.DateScraper.Application.Dmc.Services;

namespace RcCloud.DateScraper.Application.Dmc.Tests.Services;

public class DownloadDmcCalendarServiceTests
{
    [Theory]
    [InlineData("dmc_2021_2021-10-11", 223)]
    public async Task TestScrapeRaw(string fileName, int count)
    {
        var scraper = new DownloadDmcCalendarService();

        string input;
        using (var sr = new StreamReader($"Data/{fileName}.html"))
        {
            input = await sr.ReadToEndAsync();
        }

        var events = scraper.ScrapeRaw(input);

        Assert.Equal(count, events.Count);
    }

    [Fact]
    public async Task TestRetrieveBaseDocument()
    {
        var scraper = new DownloadDmcCalendarService();

        var input = await scraper.RetrieveBaseDocument(2021);

        Assert.Contains("Munzig ", input);
    }
}
