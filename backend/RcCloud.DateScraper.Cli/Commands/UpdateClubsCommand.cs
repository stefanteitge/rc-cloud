using McMaster.Extensions.CommandLineUtils;
using RcCloud.DateScraper.Application.Myrcm.Clubs.Services;
using RcCloud.DateScraper.Application.Myrcm.Common.Domain;
using RcCloud.DateScraper.Domain.Clubs;

namespace RcCloud.DateScraper.Cli.Commands;

internal class UpdateClubsCommand(ScrapeMyrcmClubs scrapeClubs, IClubRepository clubRepository)
{
    [FileExists]
    [Option("--file", CommandOptionType.SingleValue)]
    public string File { get; set; } = string.Empty;
    
    public async Task OnExecute()
    {
        clubRepository.Store(File);
        clubRepository.Load(File);
        var all = await scrapeClubs.Scrape([MyrcmCountryCode.Germany]);
        foreach (var club in all)
        {
            clubRepository.Update(club);
        }
        
        clubRepository.Store(File);

        var allFiltered = clubRepository.GetAll();
        
        foreach (var club in allFiltered)
        {
            var numbers = string.Join(", ", club.MyrcmClubNumbers);
            Console.WriteLine($"{numbers}\t{club.Name}");
        }
    }
}
