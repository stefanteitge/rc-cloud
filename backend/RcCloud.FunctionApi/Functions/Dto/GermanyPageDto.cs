using System.Collections.Generic;
using RcCloud.DateScraper.Infrastructure.Races;

namespace RcCloud.FunctionApi.Functions.Dto;

public class GermanyPageDto(string lastUpdate, List<RaceCategoryDto> categories)
{
    public string LastUpdate { get; } = lastUpdate;

    public List<RaceCategoryDto> Categories { get; } = categories;

    public static GermanyPageDto FromDocument(RacesDocument document)
    {
        var categories = new List<RaceCategoryDto>();

        CreateGlobalCategory(document, categories);
        CreateCategory(document, categories, "west");
        CreateCategory(document, categories, "east");
        CreateCategory(document, categories, "north");
        CreateCategory(document, categories, "south");
        CreateCategory(document, categories, "central");

        return new(document.LastUpdate, categories);
    }

    private static void CreateGlobalCategory(RacesDocument document, List<RaceCategoryDto> categories)
    {
        var races = document.Races.Where(r => r.Regions.Count() == 0).ToList();
        var cat = new RaceCategoryDto("global", races);
        categories.Add(cat);
    }

    private static void CreateCategory(RacesDocument document, List<RaceCategoryDto> categories, string regionName)
    {
        var races = document.Races.Where(r => r.Regions.Any(reg => reg.Id == regionName)).ToList();
        var cat = new RaceCategoryDto(regionName, races);
        categories.Add(cat);
    }
}
