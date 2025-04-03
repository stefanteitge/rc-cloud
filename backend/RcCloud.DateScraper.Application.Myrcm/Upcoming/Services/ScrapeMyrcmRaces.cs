using System.Globalization;
using System.Web;
using AngleSharp;
using AngleSharp.Dom;
using RcCloud.DateScraper.Application.Myrcm.Clubs.Services;
using RcCloud.DateScraper.Application.Myrcm.Upcoming.Domain;
using RcCloud.DateScraper.Domain;
using RcCloud.DateScraper.Domain.Clubs;
using RcCloud.DateScraper.Domain.Races;
using RcCloud.DateScraper.Domain.Series;

namespace RcCloud.DateScraper.Application.Myrcm.Upcoming.Services;

public class ScrapeMyrcmRaces(DownloadMyrcmPages downloadPages, GuessSeriesFromTitle guessSeriesFromTitle, SanitizeClubNames sanitizeClubNames)
{
    public async Task<IEnumerable<RaceMeeting>> Scrape()
    {
        var downloaded = await downloadPages.Download(DownloadFilter.GermanyOnly);
        var firstPage = await Scrape(downloaded);
        var all = new List<RaceMeeting>();
        all.AddRange(firstPage);
        int pageCount = await GetPageCount(downloaded);

        for (int i = 1; i < pageCount; i++)
        {
            var downloaded2 = await downloadPages.Download(DownloadFilter.GermanyOnly, i);
            var parsed2 = await Scrape(downloaded2);
            all.AddRange(parsed2);
        }

        sanitizeClubNames.Sanitize(all.Select(r => r.Club).OfType<Club>().ToList());
        
        return all;
    }

    private async Task<int> GetPageCount(string content)
    {
        var document = await BrowsingContext.New(Configuration.Default.WithDefaultLoader()).OpenAsync(req => req.Content(content));
        
        if (document is null)
        {
            return 0;
        }
        
        var table = document.QuerySelector(".paging");
        var lastCell = table.QuerySelectorAll("td").SkipLast(1).LastOrDefault();

        if (lastCell is not null)
        {
            return int.Parse(lastCell?.TextContent ?? "0");
        }
        
        return 0;
    }

    public async Task<IEnumerable<RaceMeeting>> Scrape(string content)
    {
        var document = await BrowsingContext.New(Configuration.Default.WithDefaultLoader()).OpenAsync(req => req.Content(content));

        if (document is null)
        {
            return [];
        }

        return await Parse2(document);
    }

    private async Task<IEnumerable<RaceMeeting>> Parse2(IDocument document)
    {
        var table = document.QuerySelector("table[id='data-table']");

        if (table is null)
        {
            return [];
        }
        
        var rows = table.QuerySelectorAll("tr");

        var meetings = new List<RaceMeeting>();
        foreach (var row in rows.Skip(1))
        {
            var cells = row.QuerySelectorAll("td");
            if (cells is null)
            {
                continue;
            }
            
            var clubNumber = GetClubNumber(cells);

            var race = new MyrcmRace(
                ParseDate(cells[4].TextContent),
                ParseDate(cells[5].TextContent),
                cells[2]?.TextContent ?? "MyRCM-Rennen",
                cells[1]?.TextContent ?? "Unbekannter Club",
                clubNumber);

            var meeting = new RaceMeeting(
                guessSeriesFromTitle.Guess(race.Title),
                SeasonReference.Current,
                race.DateEnd, 
                race.Club, // XXX club is not sanitized here
                race.Title, 
                [],
                new Club(race.Club, [], null, clubNumber));
            
            meetings.Add(meeting);
        }

        return meetings;
    }

    private static int GetClubNumber(IHtmlCollection<IElement> cells)
    {
        var url = cells[1].QuerySelector("a").Attributes["href"].Value;
        var hostLink = new Url(url);
        var clubNumber = int.Parse(HttpUtility.ParseQueryString(hostLink.Query).Get("dId[O]"));
        return clubNumber;
    }

    private DateOnly ParseDate(string input)
    {
        return DateOnly.ParseExact(input, "dd.MM.yyyy", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
    }
}