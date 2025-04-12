using RcCloud.DateScraper.Domain.Clubs;

namespace RcCloud.FunctionApi.Clubs.Dto;

public class ClubDto(
    string name,
    List<string> aliases,
    int? dmcClubNumber)
{
    public string Name { get; } = name;

    public List<string> Aliases { get; } = aliases;
    
    public int? DmcClubNumber { get; } = dmcClubNumber;
    
    public static ClubDto FromDomain(Club club) => new ClubDto(club.Name, club.Aliases,  club.DmcClubNumber);
}
