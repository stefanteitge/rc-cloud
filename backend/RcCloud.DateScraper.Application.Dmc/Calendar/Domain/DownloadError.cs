using FluentResults;

namespace RcCloud.DateScraper.Application.Dmc.Calendar.Domain;

internal class DownloadError(string url, string message) : Error(message)
{
    public string Url { get; } = url;
}
