using AngleSharp.Dom;
using RcCloud.DateScraper.Domain.Races;
using RcCloud.DateScraper.Domain.Series;

namespace RcCloud.DateScraper.Application.Rck.Services;

public class ScrapeKleinserieRaces : AbstractRckService
{
    private readonly Url BaseUrl = new("https://kleinserie.rck-solutions.de/indexgo.php");

    private readonly SeriesReference Series = new SeriesReference("kleinserie");

    public async Task<List<RaceMeeting>> Scrape()
    {
        return await Parse(Series, BaseUrl, "KleinSerie");
    }

    public async Task<List<RaceMeeting>> Parse(string content)
    {
        return await Scrape(Series, content, "KleinSerie");
    }
}
