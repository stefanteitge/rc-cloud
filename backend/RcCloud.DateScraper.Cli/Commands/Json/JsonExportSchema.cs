using RcCloud.DateScraper.Domain;

namespace RcCloud.DateScraper.Cli.Commands.Json;

public class JsonExportSchema(DateTimeOffset retrievedDate, List<RaceMeeting> raceMeetings)
{
    public DateTimeOffset retrievedDate { get; } = retrievedDate;
    
    public List<RaceMeeting> raceMeetings { get; } = raceMeetings;
}