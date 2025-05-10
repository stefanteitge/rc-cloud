using RcCloud.DateScraper.Application.Rck.Services;
using RcCloud.DateScraper.Domain.Regions;

namespace RcCloud.DateScraper.Application.Rck.Tests;

public class ScrapeLrpOffroadRacesTests
{
    public const string DataSet1File = "Data/offroad_2025-05-10.html";
    
    [Fact]
    public async Task Parse_WithDataset1_HasAllEvents()
    {
        // Arrange
        var content = File.ReadAllText(DataSet1File);
        var sut = new ScrapeChallengeRaces();

        // Act
        var result = await sut.Parse(content);


        // Assert
        Assert.Equal(7, result.Count());
    }

    [Fact]
    public async Task Parse_WithDataset1_CorrectSecondEvent()
    {
        // Arrange
        var content = File.ReadAllText(DataSet1File);
        var sut = new ScrapeChallengeRaces();

        // Act
        var result = await sut.Parse(content);


        // Assert
        var secondRace = result.Skip(1).First();
        Assert.Equal("Gunzenhausen", secondRace.Location);
        Assert.Equal(new DateOnly(2025,6,1), secondRace.Date);
        Assert.Equal(RegionReference.South, secondRace.Regions.First());
    }
}
