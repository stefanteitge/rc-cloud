namespace RcCloud.DateScraper.Domain.Series;

public record SeasonReference(string Id)
{
    public static SeasonReference Current = new("current");
}
