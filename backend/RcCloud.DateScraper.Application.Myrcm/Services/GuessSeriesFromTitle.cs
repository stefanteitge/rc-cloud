using RcCloud.DateScraper.Domain;

namespace RcCloud.DateScraper.Application.Myrcm.Services;

public class GuessSeriesFromTitle
{
    public SeriesReference[] Guess(string raceTitle)
    {
        var seriess = new List<SeriesReference>();

        if (IsInTitle(raceTitle, "Hudy Series"))
        {
            seriess.Add(SeriesReference.Hudy);
        }
        
        if (IsInTitle(raceTitle, "Tamiya Euro"))
        {
            seriess.Add(SeriesReference.Tamiya);
        }
        
        if (IsInTitle(raceTitle, "ostmasters"))
        {
            seriess.Add(new SeriesReference("ostmasters"));
        }
        
        if (IsInTitle(raceTitle, "jumpstart"))
        {
            seriess.Add(new SeriesReference("jumpstart"));
        }
        
        if (IsInTitle(raceTitle, "ElbeCup"))
        {
            seriess.Add(new SeriesReference("elbecup"));
        }
        
        if (seriess.Count > 0)
        {
            return seriess.ToArray();
        }
        
        return [new SeriesReference("myrcm")];
    }

    private bool IsInTitle(string raceTitle, string subString)
    {
        return raceTitle.Contains(subString, StringComparison.InvariantCultureIgnoreCase);
    }
}