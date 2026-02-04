using FluentResults;
using RcCloud.DateScraper.Application.Dmc.Calendar.Domain;
using RcCloud.DateScraper.Application.Dmc.Common.Domain;
using RcCloud.DateScraper.Domain.Clubs;
using RcCloud.DateScraper.Domain.Races;
using RcCloud.DateScraper.Domain.Regions;
using RcCloud.DateScraper.Domain.Series;

namespace RcCloud.DateScraper.Application.Dmc.Calendar.Services;

public class ScrapeDmcRaces(DownloadDmcCalendar download, IClubFileRepository clubFileRepository, GuessSeries guessSeries)
{
    public async Task<Result<List<RaceMeeting>>> Scrape(int year)
    {
        var scrapeResult = await download.Scrape(year);

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

    public async Task<Result<List<RaceMeeting>>> ScrapeThisAndNextYear()
    {
        var now = DateTime.UtcNow;
        // Use a const for the July 1st cutoff
        var nextYearStartDate = new DateTime(now.Year, 7, 1);
        int currentYear = now.Year;
        var resultCurrent = await Scrape(currentYear);
        if (resultCurrent.IsFailed)
        {
            return Result.Fail(resultCurrent.Errors);
        }

        // Only scrape next year if after July 1
        if (now >= nextYearStartDate)
        {
            var resultNext = await Scrape(currentYear + 1);
            if (resultNext.IsFailed)
            {
                return Result.Fail(resultNext.Errors);
            }
            
            var combined = resultCurrent.Value.Concat(resultNext.Value)
                .OrderBy(r => r.Date) // Assuming RaceMeeting has a Date property
                .ToList();
            return Result.Ok(combined);
        }
        
        return resultCurrent;
    }

    private RaceMeeting MakeRaceMeeting(DmcCalendarEntry entry)
    {
        var regions = ComputeRegions(entry);

        var club = new Club(entry.Verein, [], "de", entry.OrtsvereinNummer, [], regions.FirstOrDefault());
        
        var knownClub = clubFileRepository.FindClub(entry.Verein);
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
