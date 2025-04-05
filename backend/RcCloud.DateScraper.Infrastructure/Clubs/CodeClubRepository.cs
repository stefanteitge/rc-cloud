using RcCloud.DateScraper.Domain.Clubs;
using RcCloud.DateScraper.Domain.Regions;

namespace RcCloud.DateScraper.Infrastructure.Clubs;

public class CodeClubRepository : IClubRepository
{
    private Club[] _clubs = [
        new("AMC Magdeburg e.V.", [], null, null, RegionReference.East),
        new("MC Ettlingen e.V.", [], null, null, RegionReference.Central),
        new("MC Munster e.V.", [], null, null, RegionReference.North),
        new("MCC Rhein-Ahr e.V.", ["MCC Rhein-Ahr e.V"], null, null, RegionReference.West),
        new("MRC-Leipzig e.V.", [], null, null, RegionReference.East),
        new("Modellclub Flensburg e.V.", ["Modellclub Flensburg e. V."], null, null, RegionReference.North),
        new("ORC-B Göttingen e.V.", [], null, null, RegionReference.North),
        new("RC Speedracer e.V.", ["RC Speedracer e.V. OV-055"], null, null, RegionReference.East),
    ];
    
    public Club? FindClub(string clubName)
    {
        return _clubs
            .Where(c => c.Name == clubName || c.Aliases.Contains(clubName))
            .FirstOrDefault();
    }
}
