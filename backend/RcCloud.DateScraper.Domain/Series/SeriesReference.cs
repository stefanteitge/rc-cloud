namespace RcCloud.DateScraper.Domain.Series;

public record SeriesReference(string Id)
{
    public static SeriesReference None => new("none");

    public static SeriesReference Hudy => new("hudyseries");

    public static SeriesReference Tamiya => new("tec");
    
    public static SeriesReference Tonisport => new("tos");
}
