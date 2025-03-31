namespace RcCloud.DateScraper.Domain;

public record GroupReference(string Id)
{
    public static GroupReference West = new("weet");

    public static GroupReference East = new("east");

    public static GroupReference North = new("north");

    public static GroupReference South = new("south");

    public static GroupReference Central = new("cental");
}
