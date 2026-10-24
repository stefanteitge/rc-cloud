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

/// <summary>
/// Fixtures are live responses of <c>POST https://www.myrcm.ch/de/upcoming?pId=N</c> (cType=json, ajax=true)
/// captured on 2026-10-24: the first page, the second page and the last page (19 of 469 events).
/// </summary>
public class ScrapeMyrcmRacesV2Tests
{
    private const string FirstPageFile = "Data/myrcm-v2_2026-10-24_p0.json";
    private const string SecondPageFile = "Data/myrcm-v2_2026-10-24_p1.json";
    private const string LastPageFile = "Data/myrcm-v2_2026-10-24_p18.json";

    [Fact]
    public void ParsePage_WithFirstPage_ReadsPagingInfo()
    {
        var page = MyrcmUpcomingPage.Parse(File.ReadAllText(FirstPageFile));

        Assert.Equal(469, page.RecordCount);
        Assert.Equal(1, page.FromIndex);
        Assert.Equal(25, page.ToIndex);
        Assert.False(page.IsLastPage);
        Assert.Contains("event-card", page.Html);
    }

    [Fact]
    public void ParsePage_WithLastPage_IsLastPage()
    {
        var page = MyrcmUpcomingPage.Parse(File.ReadAllText(LastPageFile));

        Assert.Equal(451, page.FromIndex);
        Assert.Equal(469, page.ToIndex);
        Assert.True(page.IsLastPage);
    }

    [Theory]
    [InlineData(FirstPageFile, 25)]
    [InlineData(SecondPageFile, 25)]
    [InlineData(LastPageFile, 19)]
    public async Task ParseCards_ReturnsAllEventsOfPage(string file, int expectedCount)
    {
        var page = MyrcmUpcomingPage.Parse(File.ReadAllText(file));

        var races = await ScrapeMyrcmRacesV2.ParseCards(page.Html);

        Assert.Equal(expectedCount, races.Count);
    }

    [Fact]
    public async Task ParseCards_WithFirstPage_ParsesEventDetails()
    {
        var page = MyrcmUpcomingPage.Parse(File.ReadAllText(FirstPageFile));

        var races = await ScrapeMyrcmRacesV2.ParseCards(page.Html);

        var race = Assert.Single(races, r => r.Title == "Tamico Cup @ MCC Fellbach");
        Assert.Equal(new DateOnly(2026, 10, 25), race.DateStart);
        Assert.Equal(new DateOnly(2026, 10, 25), race.DateEnd);
        Assert.Equal("MCC Fellbach e.V.", race.Club);
        Assert.Equal(38659, race.ClubNumber);
        Assert.Equal(MyrcmCountryCode.Germany, race.Country);
    }

    [Fact]
    public async Task ParseCards_WithSecondPage_ParsesMultiDayEventsAcrossMonths()
    {
        var page = MyrcmUpcomingPage.Parse(File.ReadAllText(SecondPageFile));

        var races = await ScrapeMyrcmRacesV2.ParseCards(page.Html);

        var race = Assert.Single(races, r => r.Title == "Fear Farm 9.0");
        Assert.Equal(new DateOnly(2026, 10, 30), race.DateStart);
        Assert.Equal(new DateOnly(2026, 11, 1), race.DateEnd);
    }

    [Fact]
    public async Task ParseCards_WithLastPage_ParsesGermanMonthNames()
    {
        var page = MyrcmUpcomingPage.Parse(File.ReadAllText(LastPageFile));

        var races = await ScrapeMyrcmRacesV2.ParseCards(page.Html);

        Assert.Equal(new DateOnly(2027, 3, 28), Assert.Single(races, r => r.Title == "Yellow Cup 4").DateStart);
        var race = Assert.Single(races, r => r.Title.StartsWith("Национален Шампионат"));
        Assert.Equal(new DateOnly(2027, 4, 30), race.DateStart);
        Assert.Equal(new DateOnly(2027, 5, 2), race.DateEnd);
    }

    [Fact]
    public async Task Scrape_WithFirstPage_FiltersCountryAndTraining()
    {
        var guessedClubNumbers = new List<int>();
        var sut = MakeSut(Mock.Of<IDownloadMyrcmPages>(), guessedClubNumbers);

        var result = (await sut.Scrape(File.ReadAllText(FirstPageFile), [MyrcmCountryCode.Germany])).ToList();

        var race = Assert.Single(result);
        Assert.Equal("Tamico Cup @ MCC Fellbach", race.Title);
        Assert.Equal(new DateOnly(2026, 10, 25), race.Date);
        Assert.Contains(race.Regions, region => region.Id == "de");
        Assert.Equal([38659], guessedClubNumbers);
    }

    [Fact]
    public async Task Scrape_WithBenelux_ReturnsOnlyBeneluxEvents()
    {
        var sut = MakeSut(Mock.Of<IDownloadMyrcmPages>());

        var result = (await sut.Scrape(
            File.ReadAllText(SecondPageFile),
            [MyrcmCountryCode.Belgium, MyrcmCountryCode.Netherlands, MyrcmCountryCode.Luxembourg])).ToList();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, race => race.Title == "Vrij rijden 16u-23u" && race.Regions.Any(r => r.Id == "be"));
        Assert.Contains(result, race => race.Title.StartsWith("Vrij rijden 16:00") && race.Regions.Any(r => r.Id == "nl"));
    }

    [Fact]
    public async Task Scrape_DownloadsPagesUntilLastPage()
    {
        var downloadPages = new Mock<IDownloadMyrcmPages>();
        downloadPages.Setup(d => d.Download(It.IsAny<DownloadFilter>(), 0)).ReturnsAsync(File.ReadAllText(FirstPageFile));
        downloadPages.Setup(d => d.Download(It.IsAny<DownloadFilter>(), 1)).ReturnsAsync(File.ReadAllText(SecondPageFile));
        downloadPages.Setup(d => d.Download(It.IsAny<DownloadFilter>(), 2)).ReturnsAsync(File.ReadAllText(LastPageFile));
        var sut = MakeSut(downloadPages.Object);

        var result = await sut.Scrape([MyrcmCountryCode.Germany]);

        Assert.Equal(1 + 5 + 4, result.Count);
        Assert.Equal("Tamico Cup @ MCC Fellbach", result[0].Title);
        Assert.Equal("Löwen-Master-Cup Lauf 8 (Endlauf)", result[^1].Title);
        Assert.Contains(result, race => race.Title.Contains("TAMIYA EURO-CUP MC Flensburg") && race.Date == new DateOnly(2027, 4, 18));
        downloadPages.Verify(d => d.Download(It.IsAny<DownloadFilter>(), It.IsAny<int>()), Times.Exactly(3));
    }

    [Fact]
    public async Task Scrape_StopsOnEmptyPage()
    {
        const string emptyPage = """{"STATUS":"OK","ID":"data","DATA":["<section class=\"event-list\">\n</section>"],"RECORDCOUNT":[469],"FROMINDEX":[1251],"TOINDEX":[1275]}""";
        var downloadPages = new Mock<IDownloadMyrcmPages>();
        downloadPages.Setup(d => d.Download(It.IsAny<DownloadFilter>(), It.IsAny<int>())).ReturnsAsync(emptyPage);
        var sut = MakeSut(downloadPages.Object);

        var result = await sut.Scrape([MyrcmCountryCode.Germany]);

        Assert.Empty(result);
        downloadPages.Verify(d => d.Download(It.IsAny<DownloadFilter>(), It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public void GetPageUrl_UsesPageIndexAsPId()
    {
        Assert.Equal("https://www.myrcm.ch/de/upcoming?pId=0", DownloadMyrcmPagesV2.GetPageUrl(0));
        Assert.Equal("https://www.myrcm.ch/de/upcoming?pId=18", DownloadMyrcmPagesV2.GetPageUrl(18));
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
        Assert.DoesNotContain(services, descriptor => descriptor.ServiceType == typeof(ScrapeMyrcmRaces));
        Assert.DoesNotContain(services, descriptor => descriptor.ServiceType == typeof(DownloadMyrcmPages));
    }

    private static ScrapeMyrcmRacesV2 MakeSut(IDownloadMyrcmPages downloadPages, List<int>? guessedClubNumbers = null)
    {
        var guessClub = new Mock<IGuessClub>();
        guessClub
            .Setup(guess => guess.Guess(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
            .Callback<string, int, string?>((_, number, _) => guessedClubNumbers?.Add(number))
            .Returns<string, int, string?>((name, number, country) =>
                new Club(name, [], country ?? "de", null, [number], RegionReference.North));

        return new ScrapeMyrcmRacesV2(
            downloadPages,
            guessClub.Object,
            new GuessSeriesFromTitle(),
            new GuessIfItIsTraining(),
            NullLogger<ScrapeMyrcmRacesV2>.Instance);
    }

    private sealed class TestServiceCollection : List<Microsoft.Extensions.DependencyInjection.ServiceDescriptor>, Microsoft.Extensions.DependencyInjection.IServiceCollection
    {
    }
}
