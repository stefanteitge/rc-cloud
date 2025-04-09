using RcCloud.DateScraper.Application.Dmc.Calendar.Services;

namespace RcCloud.DateScraper.Application.Dmc.Tests.Services;

public class DownloadDmcCalendarTests
{
    [Theory]
    [InlineData("dmc_2021_2021-10-11", 223)]
    [InlineData("dmc_2025_2025-04-09", 256)]
    public async Task TestScrapeRaw(string fileName, int count)
    {
        var scraper = new DownloadDmcCalendar();

        string input;
        using (var sr = new StreamReader($"Data/{fileName}.html"))
        {
            input = await sr.ReadToEndAsync();
        }

        var events = scraper.ScrapeRaw(input);

        Assert.Equal(count, events.Count);
    }

    [Fact]
    public async Task RetrieveBaseDocument_For2021_Succeeds()
    {
        var scraper = new DownloadDmcCalendar();

        var input = await scraper.RetrieveBaseDocument(2021);

        Assert.Contains("Munzig ", input);
    }

    [Fact]
    public async Task RetrieveBaseDocument_For2025_Succeeds()
    {
        var scraper = new DownloadDmcCalendar();

        var input = await scraper.RetrieveBaseDocument(2025);

        File.WriteAllText("dmc", input);

        Assert.Contains("ORC-B Göttingen e.V.", input);
        Assert.Contains("13.04.2025", input);
    }
}
