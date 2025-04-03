using RcCloud.DateScraper.Domain.Races;

namespace RcCloud.DateScraper.Cli.Output.Dto;

public class JsonExportSchema(DateTimeOffset retrievedDate, List<RaceMeeting> raceMeetings)
{
    public DateTimeOffset retrievedDate { get; } = retrievedDate;
    
    // TODO: this should use a DTO
    public List<RaceMeeting> raceMeetings { get; } = raceMeetings;
}