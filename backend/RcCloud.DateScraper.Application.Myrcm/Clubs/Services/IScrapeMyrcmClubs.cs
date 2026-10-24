using RcCloud.DateScraper.Application.Myrcm.Common.Domain;
using RcCloud.DateScraper.Domain.Clubs;

namespace RcCloud.DateScraper.Application.Myrcm.Clubs.Services;

public interface IScrapeMyrcmClubs
{
    Task<List<Club>> Scrape(MyrcmCountryCode[] countries);
}
