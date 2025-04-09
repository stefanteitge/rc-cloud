using Microsoft.Extensions.Logging.Abstractions;
using RcCloud.DateScraper.Application.Dmc.Calendar.Services;

namespace RcCloud.DateScraper.Application.Dmc.Tests.Services;

public class DownloadDmcCalendarTests
{
    [Theory]
    [InlineData("dmc_2021_2021-10-11", 223)]
    [InlineData("dmc_2025_2025-04-09", 256)]
    public async Task TestScrapeRaw(string fileName, int count)
    {
        var scraper = new DownloadDmcCalendar(new NullLogger<DownloadDmcCalendar>());

        string input;
        using (var sr = new StreamReader($"Data/{fileName}.html"))
        {
            input = await sr.ReadToEndAsync();
        }

        var scrapeResult = scraper.ScrapeRaw(input);

        Assert.True(scrapeResult.IsSuccess);
        Assert.Equal(count, scrapeResult.Value.Count);
    }

    [Fact]
    public async Task RetrieveBaseDocument_For2021_Succeeds()
    {
        var scraper = new DownloadDmcCalendar(new NullLogger<DownloadDmcCalendar>());

        var downloadResult = await scraper.RetrieveBaseDocument(2021);

        Assert.True(downloadResult.IsSuccess);
        Assert.Contains("Munzig ", downloadResult.Value);
    }

    [Fact]
    public async Task RetrieveBaseDocument_For2025_Succeeds()
    {
        var scraper = new DownloadDmcCalendar(new NullLogger<DownloadDmcCalendar>());

        var downloadResult = await scraper.RetrieveBaseDocument(2025);

        Assert.True(downloadResult.IsSuccess);
        Assert.Contains("ORC-B Göttingen e.V.", downloadResult.Value);
        Assert.Contains("13.04.2025", downloadResult.Value);
    }
}
