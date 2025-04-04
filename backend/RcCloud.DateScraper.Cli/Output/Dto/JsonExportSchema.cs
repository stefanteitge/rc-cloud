using RcCloud.DateScraper.Domain.Races;

namespace RcCloud.DateScraper.Cli.Output.Dto;

public class JsonExportSchema<T>(DateTimeOffset retrievedDate, T raceMeetings)
{
    public DateTimeOffset retrievedDate { get; } = retrievedDate;
    
    // TODO: this should use a DTO
    public T raceMeetings { get; } = raceMeetings;
}
