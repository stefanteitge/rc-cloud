using Microsoft.Extensions.Logging;
using RcCloud.DateScraper.Domain.Races;

namespace RcCloud.DateScraper.Application.Common.Services;

/// <summary>
/// Scrapes a single race source with its own timeout and stores the result.
/// If the source fails or times out, the last stored races of that source are used instead,
/// so one unreachable site does not break the whole compilation.
/// </summary>
public class UpdateRaceSource(IRaceCompilationRepository repo, ILogger<UpdateRaceSource> logger)
{
    public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(20);

    public async Task<List<RaceMeeting>> Update(
        string compilation,
        string source,
        Func<Task<List<RaceMeeting>>> scrape,
        TimeSpan? timeout = null)
    {
        var effectiveTimeout = timeout ?? DefaultTimeout;

        try
        {
            var races = await scrape().WaitAsync(effectiveTimeout);
            await repo.Store(races, compilation, source);
            return races;
        }
        catch (TimeoutException)
        {
            logger.LogWarning(
                "Scraping {Compilation}/{Source} timed out after {Timeout}s, using last stored races.",
                compilation,
                source,
                effectiveTimeout.TotalSeconds);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Scraping {Compilation}/{Source} failed, using last stored races.", compilation, source);
        }

        var stored = await repo.Load(compilation, source);
        return stored?.Races ?? [];
    }
}
