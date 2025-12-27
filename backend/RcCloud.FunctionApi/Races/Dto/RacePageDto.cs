using RcCloud.DateScraper.Domain.Races;
using RcCloud.DateScraper.Infrastructure.Races;

namespace RcCloud.FunctionApi.Races.Dto;

public class RacePageDto(string lastUpdate, List<RaceDateDto> dates, string? lastDmcUpdate)
{
    public static readonly string[] BeneluxCategories = ["be", "nl", "lux", "stockcar", "banger"];
    
    public static readonly string[] GermanyRegions = ["west", "east", "north", "south", "central"];
    
    public string LastUpdate { get; } = lastUpdate;

    public string? LastDmcUpdate { get; } = lastDmcUpdate;

    public List<RaceDateDto> Dates { get; } = dates;

    public static RacePageDto FromRaces(List<RaceMeeting> races, string[] regionIds, string lastUpdate, string? lastDmcUpdate)
    {
        var dates = races.Select(r => r.Date).Distinct().Order().ToList();

        var dateDtos = new List<RaceDateDto>();
        foreach (var date in dates)
        {
            var categories = new List<RaceCategoryDto>();
            var racesWithDate = races.Where(r => r.Date == date).ToList();

            CreateGlobalCategory(racesWithDate, categories, regionIds);
            foreach (var regionId in regionIds)
            {
                CreateCategory(racesWithDate, categories, regionId);    
            }

            dateDtos.Add(new RaceDateDto(date.ToString("O"), categories));
        }

        return new RacePageDto(lastUpdate, dateDtos, lastDmcUpdate);
    }

    private static void CreateGlobalCategory(List<RaceMeeting> racesOnDate, List<RaceCategoryDto> categories, string[] regionIds)
    {
        var races = racesOnDate
            .Where(r => !r.Regions.Select(reg => reg.Id).Intersect(regionIds).Any())
            .OrderBy(r => r.Location)
            .ToList();

        var cat = new RaceCategoryDto("global", races);
        categories.Add(cat);
    }

    private static void CreateCategory(List<RaceMeeting> racesOnDate, List<RaceCategoryDto> categories, string regionId)
    {
        var races = racesOnDate
            .Where(r => r.Regions.Any(reg => reg.Id == regionId))
            .OrderBy(r => r.Location)
            .ToList();

        var cat = new RaceCategoryDto(regionId, races);
        categories.Add(cat);
    }
}
