using RcCloud.DateScraper.Domain.Races;
using RcCloud.DateScraper.Infrastructure.Races;

namespace RcCloud.FunctionApi.Races.Dto;

public class RacePageDto(string lastUpdate, List<RaceDateDto> dates, string? lastDmcUpdate)
{
    public static readonly string[] BeneluxCategories = ["be", "nl", "lux", "stockcar", "banger"];
    
    public static readonly string[] GermanyCategories = ["west", "east", "north", "south", "central"];
    
    public string LastUpdate { get; } = lastUpdate;

    public string? LastDmcUpdate { get; } = lastDmcUpdate;

    public List<RaceDateDto> Dates { get; } = dates;

    public static RacePageDto FromRaces(List<RaceMeeting> races, string[] categoryNames, string lastUpdate, string? lastDmcUpdate)
    {
        var dates = races.Select(r => r.Date).Distinct().Order().ToList();

        var dateDtos = new List<RaceDateDto>();
        foreach (var date in dates)
        {
            var categories = new List<RaceCategoryDto>();
            var racesWithDate = races.Where(r => r.Date == date).ToList();

            CreateGlobalCategory(racesWithDate, categories);
            foreach (var categoryName in categoryNames)
            {
                CreateCategory(racesWithDate, categories, categoryName);    
            }

            dateDtos.Add(new RaceDateDto(date.ToString("O"), categories));
        }

        return new RacePageDto(lastUpdate, dateDtos, lastDmcUpdate);
    }

    private static void CreateGlobalCategory(List<RaceMeeting> racesOnDate, List<RaceCategoryDto> categories)
    {
        var races = racesOnDate
            .Where(r => r.Regions.Length == 0)
            .OrderBy(r => r.Location)
            .ToList();

        var cat = new RaceCategoryDto("global", races);
        categories.Add(cat);
    }

    private static void CreateCategory(List<RaceMeeting> racesOnDate, List<RaceCategoryDto> categories, string regionName)
    {
        var races = racesOnDate
            .Where(r => r.Regions.Any(reg => reg.Id == regionName))
            .OrderBy(r => r.Location)
            .ToList();

        var cat = new RaceCategoryDto(regionName, races);
        categories.Add(cat);
    }
}
