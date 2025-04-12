namespace RcCloud.FunctionApi.Functions.Dto;

public class RaceDateDto(string dateEnd, List<RaceCategoryDto> categories)
{
    public string DateEnd { get; } = dateEnd;
    public List<RaceCategoryDto> Categories { get; } = categories;
}
