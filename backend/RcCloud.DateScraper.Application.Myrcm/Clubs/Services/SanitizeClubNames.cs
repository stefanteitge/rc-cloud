using RcCloud.DateScraper.Domain.Clubs;

namespace RcCloud.DateScraper.Application.Myrcm.Clubs.Services;

public class SanitizeClubNames
{
    public void Sanitize(List<Club> clubs)
    {
        ClubNameIs(clubs, "MSC der Polizei Braunschweig im ADAC e.V.  (SK2/OV158)",
            "MSC der Polizei Braunschweig im ADAC e.V.");
        ClubNameIs(clubs, "MSC der Polizei Braunschweig im ADAC e.V. (158)",
            "MSC der Polizei Braunschweig im ADAC e.V.");
    }
    
    private void ClubNameIs(List<Club> clubs, string wrongName, string cleanName)
    {
        var wrong = clubs.Where(clubs => clubs.Name == wrongName);
        foreach (var club in wrong)
        {
            club.SetRealName(cleanName);
        }
    }
}