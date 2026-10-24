using RcCloud.DateScraper.Domain.Series;

namespace RcCloud.DateScraper.Application.Myrcm.Upcoming.Services;

public interface IGuessSeriesFromTitle
{
    SeriesReference[] Guess(string raceTitle);
}
