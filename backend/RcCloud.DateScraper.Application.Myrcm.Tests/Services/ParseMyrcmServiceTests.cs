using RcCloud.DateScraper.Application.Myrcm.Services;

namespace RcCloud.DateScraper.Application.Myrcm.Tests.Services;

public class ParseMyrcmServiceTests
{
    [Fact]
    public async Task Parse_WithDataset1_HasAllEvents()
    {
        // Arrange
        var content = File.ReadAllText("Data/myrcm_2025-04-02.html");
        var sut = new ParseMyrcmService(new DownloadMyrcmPageService());

        // Act
        var result = await sut.Parse(content);
        
        // Assert
        Assert.Equal(50, result.Count());
    }
}