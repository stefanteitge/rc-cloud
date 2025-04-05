using AngleSharp.Dom;
using RcCloud.DateScraper.Domain;
using RcCloud.DateScraper.Domain.Races;
using RcCloud.DateScraper.Domain.Series;

namespace RcCloud.DateScraper.Application.Rck.Services
{
    public class ScrapeChallengeRaces : AbstractRckService
    {
        private readonly Url BaseUrl = new Url("https://challenge.rck-solutions.de/indexgo.php");

        private readonly SeriesReference Series = new SeriesReference("challenge");

        public async Task<IEnumerable<RaceMeeting>> Parse()
        {
            return await Parse(Series, BaseUrl, "RCK-Challenge");
        }

        public async Task<IEnumerable<RaceMeeting>> Parse(string content)
        {
            return await Parse(Series, content, "RCK-Challenge");
        }
    }
}
