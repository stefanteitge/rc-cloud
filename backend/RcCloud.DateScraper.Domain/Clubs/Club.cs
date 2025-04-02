namespace RcCloud.DateScraper.Domain.Clubs;

public class Club(string name, string[] aliases, int? dmcClubNumber, int? myrcmClubNumber)
{
    public string Name { get; } = name;
    public string[] Aliases { get; } = aliases;
    public int? DmcClubNumber { get; } = dmcClubNumber;
    public int? MyrcmClubNumber { get; } = myrcmClubNumber;
}