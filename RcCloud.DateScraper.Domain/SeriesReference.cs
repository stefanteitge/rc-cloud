namespace RcCloud.DateScraper.Domain;

public record SeriesReference(string Id)
{
    public static SeriesReference None = new("none");
}
