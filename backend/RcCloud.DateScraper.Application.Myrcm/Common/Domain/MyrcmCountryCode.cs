namespace RcCloud.DateScraper.Application.Myrcm.Common.Domain;

public record class MyrcmCountryCode(int Code, string Name)
{
    public static MyrcmCountryCode Belgium => new(8, "Belgium");

    public static MyrcmCountryCode Germany => new(3, "Germany");

    public static MyrcmCountryCode Luxembourg => new(5, "Luxembourg");

    public static MyrcmCountryCode Netherlands => new(9, "Netherlands");
}
