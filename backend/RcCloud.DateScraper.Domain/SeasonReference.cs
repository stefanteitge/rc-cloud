namespace RcCloud.DateScraper.Domain;

public record SeasonReference(string Id)
{
    public static SeasonReference Current = new("current");
}
