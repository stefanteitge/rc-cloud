namespace RcCloud.DateScraper.Application.Myrcm.SubDomain;

public class MyrcmRace(DateOnly dateStart, DateOnly dateEnd, string title, string club, int clubNumber)
{
    public DateOnly DateStart { get; } = dateStart;
    
    public DateOnly DateEnd { get; } = dateEnd;
    
    public string Title { get; } = title;
    
    public string Club { get; } = club;
    
    public int? ClubNumber { get; } = clubNumber;
}