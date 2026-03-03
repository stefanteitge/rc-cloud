using RcCloud.Provider.Dmc.Calendar.Domain;

namespace RcCloud.Provider.Dmc.Calendar.Services;

public class GuessSeries
{
    public DmcSeriesReference[] Guess(DmcCalendarEntry entry)
    {
        var seriess = new List<DmcSeriesReference>();
        
        if (entry.BemerkungLauf.Contains("TOS"))
        {
            seriess.Add(DmcSeriesReference.Tonisport);;    
        }
        
        if (entry.BemerkungLauf.Contains("Elbe Cup"))
        {
            seriess.Add(new("elbecup"));    
        }
        
        if (entry.BemerkungLauf.Contains("LE Trophy"))
        {
            seriess.Add(new("letrophy"));    
        }

        if (entry.Praedikat.IsSportkreismeisterschaft())
        {
            seriess.Add(new("dmc-sm"));
        }
        
        if (entry.Praedikat.IsDeutscheMeisterschaft())
        {
            seriess.Add(new("dmc-dm"));
        }
        
        if (entry.Praedikat.IsTamiyaEurocup())
        {
            seriess.Add(DmcSeriesReference.Tamiya);
        }

        
        return seriess.ToArray();
    }
}
