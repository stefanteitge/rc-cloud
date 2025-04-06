namespace RcCloud.DateScraper.Domain.Regions;

public record RegionReference(string Id)
{
    public static RegionReference West = new("west");

    public static RegionReference East = new("east");

    public static RegionReference North = new("north");

    public static RegionReference South = new("south");

    public static RegionReference Central = new("central");

    public override string ToString() => Id;
}
