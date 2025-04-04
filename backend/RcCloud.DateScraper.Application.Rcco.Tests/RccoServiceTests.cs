namespace RcCloud.DateScraper.Application.Rcco.Tests;

public class RccoServiceTests
{
    [Fact]
    public async Task Test1()
    {
        var sut = new RccoService();

        var races = await sut.RunAsync();

        Assert.True(races.Count > 0);
    }
}
