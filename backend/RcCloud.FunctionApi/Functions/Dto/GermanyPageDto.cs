using RcCloud.DateScraper.Domain.Races;
using RcCloud.DateScraper.Infrastructure.Races;

namespace RcCloud.FunctionApi.Functions.Dto;

public class GermanyPageDto(string lastUpdate, List<RaceDateDto> dates)
{
    public string LastUpdate { get; } = lastUpdate;

    public List<RaceDateDto> Dates { get; } = dates;

    public static GermanyPageDto FromDocument(RacesDocument document)
    {
        var dates = document.Races.Select(r => r.Date).Distinct().Order().ToList();

        var dateDtos = new List<RaceDateDto>();
        foreach (var date in dates)
        {
            var categories = new List<RaceCategoryDto>();
            var races = document.Races.Where(r => r.Date == date).ToList();

            CreateGlobalCategory(races, categories);
            CreateCategory(races, categories, "west");
            CreateCategory(races, categories, "east");
            CreateCategory(races, categories, "north");
            CreateCategory(races, categories, "south");
            CreateCategory(races, categories, "central");

            dateDtos.Add(new RaceDateDto(date.ToString(), categories));
        }

        return new(document.LastUpdate, dateDtos);
    }

    private static void CreateGlobalCategory(List<RaceMeeting> racesOnDate, List<RaceCategoryDto> categories)
    {
        var races = racesOnDate
            .Where(r => r.Regions.Count() == 0)
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
