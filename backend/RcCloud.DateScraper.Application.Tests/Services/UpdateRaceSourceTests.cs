using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using RcCloud.DateScraper.Application.Common.Services;
using RcCloud.DateScraper.Domain.Races;
using RcCloud.DateScraper.Domain.Series;

namespace RcCloud.DateScraper.Application.Tests.Services;

public class UpdateRaceSourceTests
{
    private readonly Mock<IRaceCompilationRepository> repo = new();

    [Fact]
    public async Task Update_WhenScrapeSucceeds_StoresAndReturnsRaces()
    {
        var races = new List<RaceMeeting> { MakeRace("Fresh") };
        var sut = MakeSut();

        var result = await sut.Update("germany", "rcco", () => Task.FromResult(races));

        Assert.Same(races, result);
        repo.Verify(r => r.Store(races, "germany", "rcco"), Times.Once);
        repo.Verify(r => r.Load(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Update_WhenScrapeTimesOut_ReturnsLastStoredRaces()
    {
        SetupStored("germany", "rcco", MakeRace("Stored"));
        var never = new TaskCompletionSource<List<RaceMeeting>>();
        var sut = MakeSut();

        var result = await sut.Update("germany", "rcco", () => never.Task, TimeSpan.FromMilliseconds(50));

        Assert.Equal("Stored", Assert.Single(result).Title);
        repo.Verify(r => r.Store(It.IsAny<List<RaceMeeting>>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Update_WhenScrapeThrows_ReturnsLastStoredRaces()
    {
        SetupStored("germany", "lrp", MakeRace("Stored"));
        var sut = MakeSut();

        var result = await sut.Update("germany", "lrp", () => throw new HttpRequestException("unreachable"));

        Assert.Equal("Stored", Assert.Single(result).Title);
        repo.Verify(r => r.Store(It.IsAny<List<RaceMeeting>>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Update_WhenScrapeFailsAndNothingStored_ReturnsEmpty()
    {
        var sut = MakeSut();

        var result = await sut.Update("germany", "rcco", () => Task.FromException<List<RaceMeeting>>(new InvalidOperationException()));

        Assert.Empty(result);
    }

    [Fact]
    public void Describe_IncludesExceptionChainAndHttpDetails()
    {
        var ex = new HttpRequestException(
            HttpRequestError.ConnectionError,
            "Connection refused",
            new System.Net.Sockets.SocketException(111));

        var details = UpdateRaceSource.Describe(ex);

        Assert.Contains("HttpRequestException: Connection refused", details);
        Assert.Contains("HttpRequestError=ConnectionError", details);
        Assert.Contains("---> SocketException", details);
    }

    [Fact]
    public void Describe_IncludesStatusCode()
    {
        var ex = new HttpRequestException("Forbidden", null, System.Net.HttpStatusCode.Forbidden);

        Assert.Contains("StatusCode=403", UpdateRaceSource.Describe(ex));
    }

    private UpdateRaceSource MakeSut() => new(repo.Object, NullLogger<UpdateRaceSource>.Instance);

    private void SetupStored(string compilation, string source, params RaceMeeting[] races)
    {
        repo.Setup(r => r.Load(compilation, source))
            .ReturnsAsync(new RaceCompilation(compilation, source, races.ToList(), DateTimeOffset.Now));
    }

    private static RaceMeeting MakeRace(string title) =>
        new([], SeasonReference.Current, new DateOnly(2026, 11, 1), "Club", "de", title, [], null, "Test");
}
