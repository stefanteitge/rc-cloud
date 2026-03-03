using FluentResults;

namespace RcCloud.Provider.Dmc.Calendar.Domain;

internal class DownloadError(string url, string message) : Error(message)
{
    public string Url { get; } = url;
}
