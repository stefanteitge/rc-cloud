using System.Globalization;
using System.Web;
using AngleSharp;
using AngleSharp.Dom;
using Microsoft.Extensions.Logging;
using RcCloud.DateScraper.Application.Myrcm.Common.Domain;
using RcCloud.DateScraper.Application.Myrcm.Upcoming.Domain;
using RcCloud.DateScraper.Domain.Races;
using RcCloud.DateScraper.Domain.Series;

namespace RcCloud.DateScraper.Application.Myrcm.Upcoming.Services;

public class ScrapeMyrcmRaces(
    DownloadMyrcmPages downloadPages,
    IEnhanceClub enhanceClub,
    GuessSeriesFromTitle guessSeriesFromTitle,
    GuessIfItIsTraining guessIfItIsTraining,
    ILogger logger)
{
    public async Task<List<RaceMeeting>> Scrape(MyrcmCountryCode[] countries)
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

        if (table is null)
        {
            logger.LogError("Myracm scraping: Paging table not found.");
            return 0;
        }

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

        return Parse2(document);
    }

    private IEnumerable<RaceMeeting> Parse2(IDocument document)
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
                ParseCountry(cells[3].TextContent),
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
                GetCoutryCode(race.Country),
                race.Title, 
                club?.Region is null ? [] : [club.Region],
                club,
                "Myrcm");
            
            meetings.Add(meeting);
        }

        return meetings;
    }

    private string? GetCoutryCode(MyrcmCountryCode? raceCountry)
    {
        if (raceCountry is null)
        {
            return null;
        }

        if (raceCountry.Code == MyrcmCountryCode.Belgium.Code)
        {
            return "be";
        }
        
        if (raceCountry.Code == MyrcmCountryCode.Germany.Code)
        {
            return "de";
        }
        
        if (raceCountry.Code == MyrcmCountryCode.Netherlands.Code)
        {
            return "nl";
        }
        
        if (raceCountry.Code == MyrcmCountryCode.Luxembourg.Code)
        {
            return "lu";
        }
        

        return null;
    }

    private MyrcmCountryCode? ParseCountry(string textContent)
    {
        return textContent switch
        {
            "DEU" => MyrcmCountryCode.Germany,
            "NLD" => MyrcmCountryCode.Netherlands,
            "BEL" => MyrcmCountryCode.Belgium,
            "LUX" => MyrcmCountryCode.Luxembourg,
            _ => null,
        };
    }

    private static int GetClubNumber(IHtmlCollection<IElement> cells)
    {
        var url = cells[1].QuerySelector("a")?.Attributes["href"]?.Value;
        var hostLink = new Url(url);
        var clubNumberString = HttpUtility.ParseQueryString(hostLink.Query ?? string.Empty).Get("dId[O]");

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
