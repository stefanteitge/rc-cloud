using AngleSharp;
using AngleSharp.Dom;
using RcCloud.DateScraper.Application.Myrcm.Clubs.Services;
using RcCloud.DateScraper.Application.Myrcm.Common.Domain;
using RcCloud.DateScraper.Application.Myrcm.Upcoming.Domain;
using RcCloud.DateScraper.Domain.Clubs;
using RcCloud.DateScraper.Domain.Races;
using RcCloud.DateScraper.Domain.Series;
using System.Globalization;
using System.Web;

namespace RcCloud.DateScraper.Application.Myrcm.Upcoming.Services;

public class ScrapeMyrcmRaces(
    DownloadMyrcmPages downloadPages,
    IEnhanceClub enhanceClub,
    GuessSeriesFromTitle guessSeriesFromTitle,
    GuessIfItIsTraining guessIfItIsTraining)
{
    public async Task<IEnumerable<RaceMeeting>> Scrape(MyrcmCountryCode[] countries)
    {
        var downloaded = await downloadPages.Download(new DownloadFilter(countries));
        var firstPage = await Scrape(downloaded);
        var all = new List<RaceMeeting>();
        all.AddRange(firstPage);
        int pageCount = await GetPageCount(downloaded);

        for (int i = 1; i < pageCount; i++)
        {
            var downloaded2 = await downloadPages.Download(new DownloadFilter(countries), i);
            var parsed2 = await Scrape(downloaded2);
            all.AddRange(parsed2);
        }
        
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

            if (guessIfItIsTraining.IsTraining(race.Title))
            {
                continue;
            }

            var club = enhanceClub.Guess(race.Club, clubNumber);
            
            var meeting = new RaceMeeting(
                guessSeriesFromTitle.Guess(race.Title),
                SeasonReference.Current,
                race.DateEnd, 
                club.Name,
                race.Title, 
                club?.Region is null ? [] : [club.Region],
                club,
                "Myrcm");
            
            meetings.Add(meeting);
        }

        return meetings;
    }

    private static int GetClubNumber(IHtmlCollection<IElement> cells)
    {
        var url = cells[1].QuerySelector("a")?.Attributes["href"]?.Value;
        var hostLink = new Url(url);
        var clubNumberString = HttpUtility.ParseQueryString(hostLink.Query).Get("dId[O]");

        if (string.IsNullOrEmpty(clubNumberString))
        {
            throw new ArgumentException("Club number not found in URL");
        }

        var clubNumber = int.Parse(clubNumberString);
        return clubNumber;
    }

    private DateOnly ParseDate(string input)
    {
        return DateOnly.ParseExact(input, "dd.MM.yyyy", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
    }
}
