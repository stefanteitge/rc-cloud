using RcCloud.DateScraper.Domain.Clubs;
using RcCloud.DateScraper.Domain.Regions;

namespace RcCloud.DateScraper.Infrastructure.Clubs.Entities;

public class ClubJson(
    string name,
    List<string> aliases,
    string countryCode,
    string? region,
    int? dmcClubNumber,
    List<int> myrcmClubNumbers)
{
    public string Name { get; set; } = name;

    public List<string> Aliases { get; set; } = aliases;
    
    public string countryCode { get; set; } = countryCode;

    public int? DmcClubNumber { get; set; } = dmcClubNumber;

    public List<int> MyrcmClubNumbers { get; set; } = myrcmClubNumbers;

    public string? Region { get; set; } = region;

    public Club ToDomain()
    {
        return new Club(Name, Aliases, DmcClubNumber, MyrcmClubNumbers.ToArray(), Region is null ? null : new RegionReference(Region)); 
    }
}
