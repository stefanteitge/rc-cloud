using RcCloud.DateScraper.Application.Rck.Services;
using RcCloud.DateScraper.Domain;
using RcCloud.DateScraper.Domain.Regions;

namespace RcCloud.DateScraper.Application.Rck.Tests;

public class ScrapeChallengeRacesTests
{
    [Fact]
    public async Task Parse_WithDataset1_HasAllEvents()
    {
        // Arrange
        var content = File.ReadAllText("Data/challenge_2025-03-31.html");
        var sut = new ScrapeChallengeRaces();

        // Act
        var result = await sut.Parse(content);


        // Assert
        Assert.Equal(26, result.Count());
    }

    [Fact]
    public async Task Parse_WithDataset1_CorrectFirstEvent()
    {
        // Arrange
        var content = File.ReadAllText("Data/challenge_2025-03-31.html");
        var sut = new ScrapeChallengeRaces();

        // Act
        var result = await sut.Parse(content);


        // Assert
        var first = result.First();
        Assert.Equal("Rudolstadt", first.Location);
        Assert.Equal(new DateOnly(2025,4,6), first.Date);
        Assert.Equal(RegionReference.East, first.Regions.First());
    }

    [Fact]
    public async Task Parse_WithDataset1_CorrectSuperlauf()
    {
        // Arrange
        var content = File.ReadAllText("Data/challenge_2025-03-31.html");
        var sut = new ScrapeChallengeRaces();

        // Act
        var result = await sut.Parse(content);


        // Assert
        var superlauf = result.ToList()[3];
        Assert.Equal("Superlauf in Ingolstadt", superlauf.Location);
        Assert.Equal(new DateOnly(2025, 4, 27), superlauf.Date);
        Assert.Equal(5, superlauf.Regions.Count());
    }
}