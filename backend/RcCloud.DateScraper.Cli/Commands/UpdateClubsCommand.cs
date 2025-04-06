using McMaster.Extensions.CommandLineUtils;
using RcCloud.DateScraper.Application.Dmc.Calendar.Services;
using RcCloud.DateScraper.Application.Myrcm.Clubs.Services;
using RcCloud.DateScraper.Application.Myrcm.Common.Domain;
using RcCloud.DateScraper.Domain.Clubs;

namespace RcCloud.DateScraper.Cli.Commands;

internal class UpdateClubsCommand(ScrapeDmcClubs scrapeDmcClubs, ScrapeMyrcmClubs scrapeClubs, IClubRepository clubRepository)
{
    [FileExists]
    [Option("--file", CommandOptionType.SingleValue)]
    public string File { get; set; } = string.Empty;
    
    public async Task OnExecute()
    {
        clubRepository.Load(File);
        
        var myrcm = await scrapeClubs.Scrape([MyrcmCountryCode.Germany]);
        myrcm.ForEach(club => clubRepository.Update(club));
        
        var dmc = await scrapeDmcClubs.Scrape();
        dmc.ForEach(club => clubRepository.Update(club));
        
        clubRepository.Store(File);

        var allFiltered = clubRepository.GetAll();
        
        foreach (var club in allFiltered)
        {
            var numbers = string.Join(", ", club.MyrcmClubNumbers);
            Console.WriteLine($"{club.DmcClubNumber}\t{numbers}\t{club.Name}");
        }
    }
}
