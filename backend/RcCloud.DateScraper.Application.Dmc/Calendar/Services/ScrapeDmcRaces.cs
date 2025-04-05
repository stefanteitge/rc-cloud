using RcCloud.DateScraper.Application.Dmc.Calendar.Domain;
using RcCloud.DateScraper.Application.Dmc.Common.Domain;
using RcCloud.DateScraper.Domain;
using RcCloud.DateScraper.Domain.Clubs;
using RcCloud.DateScraper.Domain.Races;
using RcCloud.DateScraper.Domain.Regions;
using RcCloud.DateScraper.Domain.Series;

namespace RcCloud.DateScraper.Application.Dmc.Calendar.Services;

public class ScrapeDmcRaces(DownloadDmcCalendar download)
{

    public async Task<IEnumerable<RaceMeeting>> Parse()
    {
        var all = await download.Scrape(2025);

        return all
            .Where(ce => ce.IsMeeting())
            .Select(ce => MakeRaceMeeting(ce));
    }

    private RaceMeeting MakeRaceMeeting(DmcCalendarEntry entry)
        => new(
            ComputeSeries(entry),
            SeasonReference.Current,
            entry.DateEnd,
            entry.Club,
            ComputeTitle(entry),
            ComputeRegions(entry),
            new Club(entry.Club, [], entry.ClubNo, null),
            "DMC");

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

        
        return seriess.ToArray();
    }

    private RegionReference[] ComputeRegions(DmcCalendarEntry a)
    {
        if (a.IsRegionMeeting(DmcRegion.Central))
        {
            return [RegionReference.Central];
        }

        if (a.IsRegionMeeting(DmcRegion.North))
        {
            return [RegionReference.North];
        }

        if (a.IsRegionMeeting(DmcRegion.West))
        {
            return [RegionReference.West];
        }

        if (a.IsRegionMeeting(DmcRegion.South))
        {
            return [RegionReference.South];
        }

        if (a.IsRegionMeeting(DmcRegion.East))
        {
            return [RegionReference.East];
        }

        return [];
    }
}
