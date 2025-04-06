using RcCloud.DateScraper.Domain.Regions;

namespace RcCloud.DateScraper.Domain.Clubs;

public class Club(string name, List<string> aliases, int? dmcClubNumber, int[] myrcmClubNumbers, RegionReference? region)
    : IEquatable<Club>
{
    public string Name { get; protected set; } = name;

    public List<string> Aliases { get; } = aliases;

    public int? DmcClubNumber { get; } = dmcClubNumber;

    public int[] MyrcmClubNumbers { get; } = myrcmClubNumbers;
    
    public RegionReference? Region { get; } = region;

    public static bool operator ==(Club lhs, Club rhs)
    {
        if (lhs is null)
        {
            if (rhs is null)
            {
                return true;
            }

            // Only the left side is null.
            return false;
        }
        // Equals handles case of null on right side.
        return lhs.Equals(rhs);
    }

    public static bool operator !=(Club lhs, Club rhs) => !(lhs == rhs);

    public override int GetHashCode() => (Name).GetHashCode();

    public bool Equals(Club? other)
    {
        if (other is null)
        {
            return false;
        }

        return Name == other.Name && DmcClubNumber == other.DmcClubNumber && MyrcmClubNumbers.Equals(other.MyrcmClubNumbers);
    }

    public override bool Equals(object? obj) => this.Equals(obj as Club);

    public void SetRealName(string cleanName)
    {
        if (cleanName != Name)
        {
            Aliases.Add(Name);
            Name = cleanName;
        }
    }
}
