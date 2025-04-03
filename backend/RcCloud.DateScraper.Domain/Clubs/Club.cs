namespace RcCloud.DateScraper.Domain.Clubs;

public class Club(string name, string[] aliases, int? dmcClubNumber, int? myrcmClubNumber)
    : IEquatable<Club>
{
    public string Name { get; } = name;
    public string[] Aliases { get; } = aliases;
    public int? DmcClubNumber { get; } = dmcClubNumber;
    public int? MyrcmClubNumber { get; } = myrcmClubNumber;

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

        return Name == other.Name && DmcClubNumber == other.DmcClubNumber && MyrcmClubNumber == other.MyrcmClubNumber;
    }

    public override bool Equals(object? obj) => this.Equals(obj as Club);
}