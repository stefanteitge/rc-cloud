using RcCloud.DateScraper.Domain.Clubs;
using RcCloud.DateScraper.Domain.Regions;

namespace RcCloud.DateScraper.Infrastructure.Clubs.Entities;

public class ClubEntity(
    string name,
    List<string> aliases,
    string countryCode,
    string? region,
    int? dmcClubNumber,
    int[] myrcmClubNumbers)
{
    public string Name { get; set; } = name;

    public List<string> Aliases { get; set; } = aliases;
    
    public string countryCode { get; set; } = countryCode;

    public int? DmcClubNumber { get; set; } = dmcClubNumber;

    public int[] MyrcmClubNumbers { get; set; } = myrcmClubNumbers;

    public string? Region { get; set; } = region;

    public Club ToDomain()
    {
        return new Club(Name, Aliases, DmcClubNumber, MyrcmClubNumbers, Region is null ? null : new RegionReference(Region)); 
    }
}
