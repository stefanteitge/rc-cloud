using RcCloud.DateScraper.Domain.Clubs;
using System.Security.Cryptography.X509Certificates;

namespace RcCloud.DateScraper.Application.Myrcm.Services;

public class ScrapeMyrcmClubs(ScrapeMyrcmRaces scrapeRaces)
{
    public async Task<IOrderedEnumerable<Club>> Scrape()
    {
        var races = await scrapeRaces.Scrape();

        var clubs = races
            .Select(r => r.Club)
            .OfType<Club>()
            .Distinct().OrderBy(c => c.Name);

        ClubNameIs(clubs, "MSC der Polizei Braunschweig im ADAC e.V.  (SK2/OV158)",
            "MSC der Polizei Braunschweig im ADAC e.V.");
        ClubNameIs(clubs, "MSC der Polizei Braunschweig im ADAC e.V. (158)",
            "MSC der Polizei Braunschweig im ADAC e.V.");

        return clubs;
    }

    private void ClubNameIs(IOrderedEnumerable<Club> clubs, string wrongName, string cleanName)
    {
        var wrong = clubs.Where(clubs => clubs.Name == wrongName);
        foreach (var club in wrong)
        {
            club.SetRealName(cleanName);
        }
    }
}