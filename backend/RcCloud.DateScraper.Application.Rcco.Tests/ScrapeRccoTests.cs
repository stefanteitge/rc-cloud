using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using RcCloud.DateScraper.Application.Rcco.Services;
using RcCloud.DateScraper.Application.Rcco.Services.Impl;
using RcCloud.DateScraper.Domain.Clubs;

namespace RcCloud.DateScraper.Application.Rcco.Tests;

public class ScrapeRccoTests
{
    [Fact]
    public void Parse_WithDataSet1_Succeeds()
    {
        var input = File.ReadAllText("Data/rcco_2025-04-06.html");
        
        var sut = MakeSut();

        var races = sut.Scrape(input);

        Assert.Equal(33, races.Count);
        Assert.Equal("8. MSC - Pokal Saison 2024/2025", races.First().Title);
    }

    [Fact]
    public void Parse_WithoutTables_ThrowsWithContentPreview()
    {
        var sut = MakeSut();

        var ex = Assert.Throws<InvalidOperationException>(() => sut.Scrape("<html><body>Access denied</body></html>"));

        Assert.Contains("no tables", ex.Message);
        Assert.Contains("Access denied", ex.Message);
    }

    [Fact]
    public void CreateClient_SendsBrowserLikeHeaders()
    {
        using var client = ScrapeRcco.CreateClient();

        Assert.Equal(ScrapeRcco.UserAgent, client.DefaultRequestHeaders.UserAgent.ToString());
        Assert.StartsWith("Mozilla/5.0", client.DefaultRequestHeaders.UserAgent.ToString());
        Assert.Contains("text/html", client.DefaultRequestHeaders.Accept.ToString());
        Assert.Contains("de-DE", client.DefaultRequestHeaders.AcceptLanguage.ToString());
        Assert.Equal(TimeSpan.FromSeconds(15), client.Timeout);
    }

    private static ScrapeRcco MakeSut() =>
        new(new GuessClub(new Mock<IClubFileRepository>().Object), NullLogger<ScrapeRcco>.Instance);
}
