using FluentResults;

namespace RcCloud.Provider.Dmc.Calendar.Domain;
internal class ScrapeError(string message) : Error(message)
{
}
