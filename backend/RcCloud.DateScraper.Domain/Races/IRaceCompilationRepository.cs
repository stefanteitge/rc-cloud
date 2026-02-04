namespace RcCloud.DateScraper.Domain.Races;

public interface IRaceCompilationRepository
{
    Task<RaceCompilation?> Load(string compilation, string source);

    Task<bool> Store(List<RaceMeeting> races, string compilation, string source);
}
