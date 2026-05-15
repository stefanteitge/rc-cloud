using RcCloud.DateScraper.Domain.Clubs;

namespace RcCloud.WebApi.Clubs.Dto;

public class ClubDto(string name, int? dmcClubNumber, string? region)
{
    public string Name { get; } = name;

    public int? DmcClubNumber { get; } = dmcClubNumber;

    public string? Region { get; } = region;

    public static ClubDto FromDomain(Club club) => new ClubDto(club.Name, club.DmcClubNumber, club.Region?.Id);
}
