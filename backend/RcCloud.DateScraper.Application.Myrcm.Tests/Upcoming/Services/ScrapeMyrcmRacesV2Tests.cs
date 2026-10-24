using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using RcCloud.DateScraper.Application.Myrcm;
using RcCloud.DateScraper.Application.Myrcm.Clubs.Services;
using RcCloud.DateScraper.Application.Myrcm.Common.Domain;
using RcCloud.DateScraper.Application.Myrcm.Upcoming.Services;
using RcCloud.DateScraper.Application.Myrcm.Upcoming.Services.V2;
using RcCloud.DateScraper.Domain.Clubs;
using RcCloud.DateScraper.Domain.Regions;

namespace RcCloud.DateScraper.Application.Myrcm.Tests.Upcoming.Services;

public class ScrapeMyrcmRacesV2Tests
{
    [Fact]
    public async Task Scrape_ParsesUpcomingPageAndFiltersRequestedCountries()
    {
        const string content = """
            <table>
              <thead><tr><th>Date</th><th>Event</th><th>Organizer</th><th>Country</th></tr></thead>
              <tbody>
                <tr>
                  <td>14.06.2026 - 15.06.2026</td>
                  <td><a href="/event/1">German Cup</a></td>
                  <td><a href="/myrcm/main?dId[O]=123">Fixture Club</a></td>
                  <td>Germany</td>
                </tr>
                <tr>
                  <td>20.06.2026</td>
                  <td><a href="/event/2">Belgian Cup</a></td>
                  <td>Belgian Club</td>
                  <td>Belgium</td>
                </tr>
                <tr>
                  <td>21.06.2026</td>
                  <td><a href="/event/3">Training day</a></td>
                  <td>Fixture Club</td>
                  <td>Germany</td>
                </tr>
              </tbody>
            </table>
            """;
        var club = new Club("Fixture Club", [], "de", null, [123], RegionReference.North);
            var guessedClubNumber = -1;
            var guessClub = new Mock<IGuessClub>();
            guessClub
                .Setup(guess => guess.Guess(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                .Callback<string, int, string?>((_, number, _) => guessedClubNumber = number)
                .Returns(club);
        var sut = new ScrapeMyrcmRacesV2(
            Mock.Of<IDownloadMyrcmPages>(),
            guessClub.Object,
            new GuessSeriesFromTitle(),
            new GuessIfItIsTraining(),
            NullLogger<ScrapeMyrcmRacesV2>.Instance);

        var result = (await sut.Scrape(content, [MyrcmCountryCode.Germany])).ToList();

        var race = Assert.Single(result);
        Assert.Equal("German Cup", race.Title);
        Assert.Equal(new DateOnly(2026, 6, 15), race.Date);
        Assert.Contains(race.Regions, region => region.Id == "de");
        Assert.Equal(123, guessedClubNumber);
    }

    [Fact]
    public async Task Scrape_ReturnsAll468EventsFromSingleUpcomingPage()
    {
        const int eventCount = 468;
        var content = new System.Text.StringBuilder("""
            <table>
              <thead><tr><th>Date</th><th>Event</th><th>Organizer</th><th>Country</th></tr></thead>
              <tbody>
            """);
        for (var index = 1; index <= eventCount; index++)
        {
            content.Append($"""
                <tr><td>14.06.2026</td><td><a href="/event/{index}">Race {index}</a></td><td>Fixture Club</td><td>Germany</td></tr>
                """);
        }

        content.Append("</tbody></table>");
        var downloadPages = new Mock<IDownloadMyrcmPages>();
        downloadPages.Setup(download => download.Download(It.IsAny<DownloadFilter>(), 0))
            .ReturnsAsync(content.ToString());
        var club = new Club("Fixture Club", [], "de", null, [], RegionReference.North);
        var guessClub = new Mock<IGuessClub>();
        guessClub.Setup(guess => guess.Guess("Fixture Club", 0, "de")).Returns(club);
        var sut = new ScrapeMyrcmRacesV2(
            downloadPages.Object,
            guessClub.Object,
            new GuessSeriesFromTitle(),
            new GuessIfItIsTraining(),
            NullLogger<ScrapeMyrcmRacesV2>.Instance);

        var result = await sut.Scrape([MyrcmCountryCode.Germany]);

        Assert.Equal(eventCount, result.Count);
        Assert.Equal("Race 1", result[0].Title);
        Assert.Equal($"Race {eventCount}", result[^1].Title);
        downloadPages.Verify(download => download.Download(It.IsAny<DownloadFilter>(), 0), Times.Once);
    }

    [Fact]
    public void AddMyrcmV2_RegistersV2ImplementationsBehindInterfaces()
    {
        var services = new TestServiceCollection();

        services.AddMyrcmV2();

        Assert.Equal(
            typeof(DownloadMyrcmPagesV2),
            services.Last(descriptor => descriptor.ServiceType == typeof(IDownloadMyrcmPages)).ImplementationType);
        Assert.Equal(
            typeof(ScrapeMyrcmRacesV2),
            services.Last(descriptor => descriptor.ServiceType == typeof(IScrapeMyrcmRaces)).ImplementationType);
        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(IScrapeMyrcmClubs));
        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(ScrapeMyrcmRaces));
        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(DownloadMyrcmPages));
    }

    private sealed class TestServiceCollection : List<Microsoft.Extensions.DependencyInjection.ServiceDescriptor>, Microsoft.Extensions.DependencyInjection.IServiceCollection
    {
    }
}
