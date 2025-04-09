using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using RcCloud.DateScraper.Application.Myrcm.Upcoming.Services;
using RcCloud.DateScraper.Application.Myrcm.Upcoming.Services.Impl;
using RcCloud.DateScraper.Domain.Clubs;

namespace RcCloud.DateScraper.Application.Myrcm.Tests.Upcoming.Services;

public class ScrapeMyrcmRacesTests
{
    [Fact]
    public async Task Parse_WithDataset1_HasAllEvents()
    {
        // Arrange
        var content = File.ReadAllText("Data/myrcm_2025-04-02.html");
        var sut = new ScrapeMyrcmRaces(
            new DownloadMyrcmPages(),
            new EnhanceClub(new Mock<IClubRepository>().Object),
            new GuessSeriesFromTitle(),
            new GuessIfItIsTraining(),
            new NullLogger<ScrapeMyrcmRaces>());

        // Act
        var result = await sut.Scrape(content);
        
        // Assert
        Assert.Equal(39, result.Count());
    }
}
