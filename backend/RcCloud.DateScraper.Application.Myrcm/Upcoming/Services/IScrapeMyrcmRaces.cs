using RcCloud.DateScraper.Application.Myrcm.Common.Domain;
using RcCloud.DateScraper.Domain.Races;

namespace RcCloud.DateScraper.Application.Myrcm.Upcoming.Services;

public interface IScrapeMyrcmRaces
{
    Task<List<RaceMeeting>> Scrape(MyrcmCountryCode[] countries);
}
