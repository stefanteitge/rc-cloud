using RcCloud.Provider.Dmc.Common.Domain;

namespace RcCloud.Provider.Dmc.Calendar.Domain;

public class Praedikat(string value)
{
    public string Value { get; } = value;

    public bool IsSportkreismeisterschaft()
    {
        return Value.StartsWith("SM") && Value.EndsWith("DMC");
    }

    public bool IsFreundschaftsrennen()
    {
        return Value.StartsWith("FR") && Value.EndsWith("DMC");
    }

    public bool IsDeutscheMeisterschaft()
    {
        return (Value.StartsWith("DM") || Value.StartsWith("ODM")) && Value.EndsWith("DMC");
    }

    public bool IsShCup()
    {
        return Value.StartsWith("CUPSH") && Value.EndsWith("DMC");
    }

    public bool IsTamiyaEurocup()
    {
        return Value.StartsWith("MP-TA") && Value.EndsWith("DMC");
    }

    public bool IsRegionMeeting(DmcRegion regionNumber) =>
        Value == $"FR{(int)regionNumber}DMC"
        || Value == $"SM{(int)regionNumber}DMC";
}
