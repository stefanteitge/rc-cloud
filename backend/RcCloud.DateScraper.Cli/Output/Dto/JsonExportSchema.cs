namespace RcCloud.DateScraper.Cli.Output.Dto;

public class JsonExportSchema<T>(DateTimeOffset lastUpdate, T races)
{
    public DateTimeOffset LastUpdate { get; } = lastUpdate;
    
    // TODO: this should use a DTO
    public T Races { get; } = races;
}
