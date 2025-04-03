using RcCloud.DateScraper.Application.Myrcm.Clubs.Services;
using RcCloud.DateScraper.Application.Myrcm.Upcoming.Services;

namespace RcCloud.DateScraper.Application.Myrcm.Tests.Services;

public class ScrapeMyrcmRacesTests
{
    [Fact]
    public async Task Parse_WithDataset1_HasAllEvents()
    {
        // Arrange
        var content = File.ReadAllText("Data/myrcm_2025-04-02.html");
        var sut = new ScrapeMyrcmRaces(new DownloadMyrcmPages(), new GuessSeriesFromTitle(), new SanitizeClubNames());

        // Act
        var result = await sut.Scrape(content);
        
        // Assert
        Assert.Equal(50, result.Count());
    }
}