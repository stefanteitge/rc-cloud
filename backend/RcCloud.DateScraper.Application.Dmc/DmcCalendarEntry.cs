namespace RcCloud.DateScraper.Application.Dmc;

public class DmcCalendarEntry
{
    private string[] NonEventTypes = ["SRLDMC", "SKTDMC", "PRAESDMC"];

    public DateTimeOffset? Timestamp { get; set; }

    public DateOnly DateStart { get; init; }
    
    public DateOnly DateEnd { get; set; }
    
    public string Type { get; init; }
    
    public string[] Classes { get; init; }
    
    public int? ClubNo { get; set; }
    
    public string Club { get; set; }
    
    public string Location { get; set; }
    
    public string Comment { get; set; }
    
    public string[] Announcement { get; set; }
    
    public string[] Entering { get; set; }
    
    public string[] Results { get; set; }
    
    public string[] Related { get; set; }
   
    public bool IsRegionMeeting(DmcRegion regionNumber) => 
        Type == $"FR{(int)regionNumber}DMC" 
        || Type == $"SM{(int)regionNumber}DMC";

    public bool IsMeeting() => !NonEventTypes.Contains(Type);
}