using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using RcCloud.DateScraper.Application.Myrcm.Upcoming.Services;
using RcCloud.DateScraper.Application.Myrcm.Upcoming.Services.Impl;
using RcCloud.DateScraper.Domain.Clubs;

namespace RcCloud.DateScraper.Application.Myrcm.Tests.Upcoming.Services;

public class ScrapeMyrcmRacesTests
{
    private const string DataSet1File = "Data/myrcm_2025-04-02.html";
    
    [Fact]
    public async Task Parse_WithDataset1_HasAllEvents()
    {
        // Arrange
        var content = File.ReadAllText(DataSet1File);
        
        // Act
        var sut = MakeSut();
        var result = await sut.Scrape(content);
        
        // Assert
        Assert.Equal(39, result.Count());
    }
    
    [Fact]
    public async Task Parse_WithDataset1_AddCountryAsRegion()
    {
        // Arrange
        var content = File.ReadAllText(DataSet1File);
        
        // Act
        var sut = MakeSut();
        var result = await sut.Scrape(content);
        
        // Assert
        Assert.Contains(result.First().Regions, r => r.Id == "de");
    }

    private static ScrapeMyrcmRaces MakeSut() =>
        new(
            new DownloadMyrcmPages(),
            new GuessClub(new Mock<IClubRepository>().Object),
            new GuessSeriesFromTitle(),
            new GuessIfItIsTraining(),
            new NullLogger<ScrapeMyrcmRaces>());
}
