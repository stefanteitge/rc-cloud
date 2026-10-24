using System.Diagnostics;
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
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var races = await scrape().WaitAsync(effectiveTimeout);
            await repo.Store(races, compilation, source);
            return races;
        }
        catch (TimeoutException ex) when (ex.InnerException is null)
        {
            // Thrown by WaitAsync: the scraper itself did not finish or fail in time.
            logger.LogWarning(
                "Scraping {Compilation}/{Source} timed out after {ElapsedMs}ms (limit {TimeoutMs}ms), using last stored races.",
                compilation,
                source,
                stopwatch.ElapsedMilliseconds,
                effectiveTimeout.TotalMilliseconds);
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                ex,
                "Scraping {Compilation}/{Source} failed after {ElapsedMs}ms, using last stored races. {Details}",
                compilation,
                source,
                stopwatch.ElapsedMilliseconds,
                Describe(ex));
        }

        var stored = await repo.Load(compilation, source);
        logger.LogInformation(
            "Using {Count} stored races for {Compilation}/{Source} from {LastUpdate}.",
            stored?.Races.Count ?? 0,
            compilation,
            source,
            stored?.LastUpdate ?? "never");

        return stored?.Races ?? [];
    }

    /// <summary>
    /// Flattens the exception chain into one line, including HTTP status and error kind where available.
    /// </summary>
    public static string Describe(Exception ex)
    {
        var parts = new List<string>();
        for (var current = ex; current is not null; current = current.InnerException)
        {
            var part = $"{current.GetType().Name}: {current.Message}";
            if (current is HttpRequestException http)
            {
                part += $" [HttpRequestError={http.HttpRequestError}";
                if (http.StatusCode is not null)
                {
                    part += $", StatusCode={(int)http.StatusCode}";
                }

                part += "]";
            }

            parts.Add(part);
        }

        return string.Join(" ---> ", parts);
    }
}
