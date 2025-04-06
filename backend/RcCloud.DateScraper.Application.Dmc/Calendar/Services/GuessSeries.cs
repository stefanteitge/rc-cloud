using RcCloud.DateScraper.Application.Dmc.Calendar.Domain;
using RcCloud.DateScraper.Domain.Series;

namespace RcCloud.DateScraper.Application.Dmc.Calendar.Services;

public class GuessSeries
{
    public SeriesReference[] Guess(DmcCalendarEntry entry)
    {
        var seriess = new List<SeriesReference>();
        
        if (entry.Comment.Contains("TOS"))
        {
            seriess.Add(SeriesReference.Tonisport);;    
        }
        
        if (entry.Comment.Contains("Elbe Cup"))
        {
            seriess.Add(new("elbecup"));    
        }
        
        if (entry.Comment.Contains("LE Trophy"))
        {
            seriess.Add(new("letrophy"));    
        }

        if (entry.IsSportkreismeisterschaft())
        {
            seriess.Add(new("dmc-sm"));
        }
        
        if (entry.IsDeutscheMeisterschaft())
        {
            seriess.Add(new("dmc-dm"));
        }
        
        if (entry.IsTamiyaEurocup())
        {
            seriess.Add(SeriesReference.Tamiya);
        }

        
        return seriess.ToArray();
    }
}
