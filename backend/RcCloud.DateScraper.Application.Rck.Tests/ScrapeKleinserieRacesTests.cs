using RcCloud.DateScraper.Application.Rck.Services;
using RcCloud.DateScraper.Domain;
using RcCloud.DateScraper.Domain.Regions;

namespace RcCloud.DateScraper.Application.Rck.Tests;

public class ScrapeKleinserieRacesTests
{
    public const string Dataset1Path = "Data/kleinserie_2025-03-31.html";

    [Fact]
    public async Task Parse_WithDataset1_HasAllEvents()
    {
        // Arrange
        var content = File.ReadAllText(Dataset1Path);
        var sut = new ScrapeKleinserieRaces();

        // Act
        var result = await sut.Parse(content);

        // Assert
        Assert.Equal(24, result.Count());
    }

    [Fact]
    public async Task Parse_WithDataset1_CorrectFirstEvent()
    {
        // Arrange
        var content = File.ReadAllText(Dataset1Path);
        var sut = new ScrapeChallengeRaces();

        // Act
        var result = await sut.Parse(content);

        // Assert
        var first = result.First();
        Assert.Equal("Hennef", first.Location);
        Assert.Equal(new DateOnly(2025, 4, 13), first.Date);
        Assert.Equal(RegionReference.West, first.Regions.First());
    }
}