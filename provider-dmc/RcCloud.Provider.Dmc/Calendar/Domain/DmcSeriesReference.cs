namespace RcCloud.Provider.Dmc.Calendar.Domain;

public record DmcSeriesReference(string Id)
{
    public static DmcSeriesReference None => new("none");

    public static DmcSeriesReference Hudy => new("hudyseries");

    public static DmcSeriesReference Tamiya => new("tec");

    public static DmcSeriesReference Tonisport => new("tos");
}
