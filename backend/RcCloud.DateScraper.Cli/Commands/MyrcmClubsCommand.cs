using RcCloud.DateScraper.Application.Myrcm.Services;

namespace RcCloud.DateScraper.Cli.Commands;

internal class MyrcmClubsCommand(ScrapeMyrcmClubs scrapeClubs)
{
    public async Task OnExecute()
    {
        var all = await scrapeClubs.Scrape();
        foreach (var club in all)
        {
            Console.WriteLine($"{club.MyrcmClubNumber}\t{club.Name}");
        }
    }
}
