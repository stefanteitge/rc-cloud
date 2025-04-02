namespace RcCloud.DateScraper.Application.Dmc;

public class DmcCalendarEntry(DateOnly dateStart, DateOnly dateEnd, string type, string[] classes, int? clubNo, string club, string location)
{
    private string[] NonEventTypes = ["SRLDMC", "SKTDMC", "PRAESDMC"];

    public DateOnly DateStart => dateStart;

    public DateOnly DateEnd => dateEnd;

    public string Type => type;

    public string[] Classes => classes;

    public int? ClubNo => clubNo;

    public string Club => club;

    public string Location => location;
    
    public string Comment { get; set; }
    
    public string[] Announcement { get; set; }
    
    public string[] Entering { get; set; }
    
    public string[] Results { get; set; }
    
    public string[] Related { get; set; }
   
    public bool IsRegionMeeting(DmcRegion regionNumber) => 
        Type == $"FR{(int)regionNumber}DMC" 
        || Type == $"SM{(int)regionNumber}DMC";

    public bool IsMeeting() => !NonEventTypes.Contains(Type);

    public bool IsSportkreismeisterschaft()
    {
        return Type.StartsWith("SM") && Type.EndsWith("DMC");
    }

    public bool IsFreundschaftsrennen()
    {
        return Type.StartsWith("FR") && Type.EndsWith("DMC");
    }
    
    public bool IsDeutscheMeisterschaft()
    {
        return Type.StartsWith("DM") && Type.EndsWith("DMC");
    }

    public bool IsShCup()
    {
        return Type.StartsWith("CUPSH") && Type.EndsWith("DMC");
    }
    
    public bool IsTamiyaEurocup()
    {
        return Type.StartsWith("MP-TA") && Type.EndsWith("DMC");
    }
}