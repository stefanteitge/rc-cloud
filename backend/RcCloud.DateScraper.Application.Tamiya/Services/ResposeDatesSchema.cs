namespace RcCloud.DateScraper.Application.Tamiya.Services;

public class ResposeDatesSchema
{
    public List<ResponseDates> Dates { get; set; }
    
    public class ResponseDates
    {
        public string DateEnd { get; set; }
        
        public string Title { get; set; }
        
        public string Location { get; set; }
        
        public string ClubName { get; set; }
    }
}
