using AngleSharp.Dom;
using RcCloud.DateScraper.Domain.Races;
using RcCloud.DateScraper.Domain.Series;

namespace RcCloud.DateScraper.Application.Rck.Services
{
    public class ScrapeKleinserieRaces : AbstractRckService
    {
        private readonly Url BaseUrl = new("https://kleinserie.rck-solutions.de/indexgo.php");

        private readonly SeriesReference Series = new SeriesReference("kleinserie");

        public async Task<IEnumerable<RaceMeeting>> Parse()
        {
            return await Parse(Series, BaseUrl);
        }

        public async Task<IEnumerable<RaceMeeting>> Parse(string content)
        {
            return await Parse(Series, content);
        }
    }
}
