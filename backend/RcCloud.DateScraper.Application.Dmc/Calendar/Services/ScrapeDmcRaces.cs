using FluentResults;
using RcCloud.DateScraper.Application.Dmc.Calendar.Domain;
using RcCloud.DateScraper.Application.Dmc.Common.Domain;
using RcCloud.DateScraper.Domain.Clubs;
using RcCloud.DateScraper.Domain.Races;
using RcCloud.DateScraper.Domain.Regions;
using RcCloud.DateScraper.Domain.Series;

namespace RcCloud.DateScraper.Application.Dmc.Calendar.Services;

public class ScrapeDmcRaces(DownloadDmcCalendar download, IClubRepository clubRepository, GuessSeries guessSeries)
{
    public async Task<Result<List<RaceMeeting>>> Scrape()
    {
        var scrapeResult = await download.Scrape(2025);

        if (scrapeResult.IsFailed)
        {
            return Result.Fail(scrapeResult.Errors);
        }

        var races = scrapeResult.Value
            .Where(ce => ce.IsMeeting())
            .Select(ce => MakeRaceMeeting(ce))
            .ToList();

        return Result.Ok(races);
    }

    private RaceMeeting MakeRaceMeeting(DmcCalendarEntry entry)
    {
        var regions = ComputeRegions(entry);

        var club = new Club(entry.Verein, [], "de", entry.OrtsvereinNummer, [], regions.FirstOrDefault());
        
        var knownClub = clubRepository.FindClub(entry.Verein);
        if (knownClub is not null)
        {
            club = new Club(
                knownClub.Name,
                knownClub.Aliases,
                "de",
                knownClub.DmcClubNumber ?? entry.OrtsvereinNummer, 
                knownClub.MyrcmClubNumbers, knownClub.Region ?? club.Region);

            if (club.Region is not null && !regions.Contains(club.Region))
            {
                regions.Add(club.Region);
            }
        }
        
        return new(
            guessSeries.Guess(entry),
            SeasonReference.Current,
            entry.Ende,
            club.Name,
            "de",
            ComputeTitle(entry),
            [.. regions],
            club,
            "DMC");
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

    private List<RegionReference> ComputeRegions(DmcCalendarEntry a)
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
