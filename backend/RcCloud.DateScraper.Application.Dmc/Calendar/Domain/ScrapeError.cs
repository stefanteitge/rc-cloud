using FluentResults;

namespace RcCloud.DateScraper.Application.Dmc.Calendar.Domain;
internal class ScrapeError(string message) : Error(message)
{
}
