using AngleSharp.Dom;
using RcCloud.DateScraper.Domain.Races;
using RcCloud.DateScraper.Domain.Series;

namespace RcCloud.DateScraper.Application.Rck.Services;

public class ScrapeLrpOffroadRaces : AbstractRckService
{
    private readonly Url BaseUrl = new Url("http://www.lrp-challenge.de/nenntool/indexgo.php");

    private readonly SeriesReference Series = new SeriesReference("lrp-offroad");

    public async Task<List<RaceMeeting>> Scrape()
    {
        return await Parse(Series, BaseUrl, "LRP-Offroad-Challenge");
    }

    public async Task<List<RaceMeeting>> Parse(string content)
    {
        return await Scrape(Series, content, "LRP-Offroad-Challenge");
    }
}
