using HtmlAgilityPack;
using RcCloud.DateScraper.Domain;
using System.Globalization;
using RcCloud.DateScraper.Application.Dmc.Services;

namespace RcCloud.DateScraper.Application.Dmc;

public class DmcService(DownloadDmcCalendarService download)
{

    public async Task<IEnumerable<RaceMeeting>> Parse()
    {
        var all = await download.Scrape(2025);

        return all
            .Where(ce => ce.IsMeeting())
            .Select(a => new RaceMeeting(ComputeSeries(a), SeasonReference.Current, a.DateEnd, a.Club, ComputeTitle(a),ComputeRegions(a)));
    }

    private string ComputeTitle(DmcCalendarEntry entry)
    {
        if (entry.IsSportkreismeisterschaft())
        {
            return "SM-Lauf";
        }
        
        if (entry.IsFreundschaftsrennen())
        {
            return "Freundschaftsrennen";
        }
        
        if (entry.IsDeutscheMeisterschaft())
        {
            return "Deutsche Meisterschaft";
        }
        
        if (entry.IsShCup())
        {
            return "SH-Cup";
        }
        
        if (entry.IsTamiyaEurocup())
        {
            return "Tamiya Eurocup";
        }

        return "Rennen";
    }

    private SeriesReference[] ComputeSeries(DmcCalendarEntry entry)
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
            seriess.Add(new("sm"));
        }
        
        if (entry.IsTamiyaEurocup())
        {
            seriess.Add(SeriesReference.Tamiya);
        }


        if (seriess.Count > 0)
        {
            return seriess.ToArray();
        }
        
        return [new("dmc")];
    }

    private GroupReference[] ComputeRegions(DmcCalendarEntry a)
    {
        if (a.IsRegionMeeting(DmcRegion.Central))
        {
            return [GroupReference.Central];
        }

        if (a.IsRegionMeeting(DmcRegion.North))
        {
            return [GroupReference.North];
        }

        if (a.IsRegionMeeting(DmcRegion.West))
        {
            return [GroupReference.West];
        }

        if (a.IsRegionMeeting(DmcRegion.South))
        {
            return [GroupReference.South];
        }

        if (a.IsRegionMeeting(DmcRegion.East))
        {
            return [GroupReference.East];
        }

        return [];
    }
}
