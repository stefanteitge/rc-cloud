using Moq;
using RcCloud.DateScraper.Domain.Clubs;

namespace RcCloud.DateScraper.Application.Rcco.Tests;

public class ScrapeRccoTests
{
    [Fact]
    public async Task Parse_WithDataSet1_Succeeds()
    {
        var input = File.ReadAllText("Data/rcco_2025-04-06.html");
        
        var sut = new ScrapeRcco(new Mock<IClubRepository>().Object);

        var races = await sut.Parse(input);

        Assert.Equal(33, races.Count);
        Assert.Equal("8. MSC - Pokal Saison 2024/2025", races.First().Title);
    }
}
