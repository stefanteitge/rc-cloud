using RcCloud.DateScraper.Application.Myrcm.Common.Domain;

namespace RcCloud.DateScraper.Application.Myrcm.Upcoming.Domain;

public class MyrcmRace(DateOnly dateStart, DateOnly dateEnd, MyrcmCountryCode? country, string title, string club, int clubNumber)
{
    public DateOnly DateStart { get; } = dateStart;
    
    public DateOnly DateEnd { get; } = dateEnd;
    
    public MyrcmCountryCode? Country { get; } = country;

    public string Title { get; } = title;
    
    public string Club { get; } = club;
    
    public int? ClubNumber { get; } = clubNumber;
}
