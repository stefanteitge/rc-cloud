using RcCloud.DateScraper.Domain;
using RcCloud.DateScraper.Domain.Races;

namespace RcCloud.DateScraper.Cli.Commands.Utils;

public class JsonExportSchema(DateTimeOffset retrievedDate, List<RaceMeeting> raceMeetings)
{
    public DateTimeOffset retrievedDate { get; } = retrievedDate;
    
    // TODO: this should use a DTO
    public List<RaceMeeting> raceMeetings { get; } = raceMeetings;
}