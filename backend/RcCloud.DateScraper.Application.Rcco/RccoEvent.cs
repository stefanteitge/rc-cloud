namespace RcCloud.DateScraper.Application.Rcco;

public class RccoEvent
{
    public RccoEvent()
    {
    }

    public DateTimeOffset? Timestamp { get; set; }

    public string DateStart { get; set; }
    
    public string DateEnd { get; set; }
    
    public string Type { get; set; }
    
    public string[] Classes { get; set; }
    
    public string ClubNo { get; set; }
    
    public string Club { get; set; }
    
    public string Location { get; set; }
    
    public string Comment { get; set; }
    
    public string[] Announcement { get; set; }
    
    public string[] Entering { get; set; }
    
    public string[] Results { get; set; }
    
    public string[] Related { get; set; }
    
    public string Region { get; set; }
    
    public string Series { get; set; }
}
