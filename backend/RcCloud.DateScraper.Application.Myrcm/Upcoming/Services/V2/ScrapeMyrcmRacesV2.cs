using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;
using AngleSharp;
using AngleSharp.Dom;
using Microsoft.Extensions.Logging;
using RcCloud.DateScraper.Application.Myrcm.Common.Domain;
using RcCloud.DateScraper.Application.Myrcm.Upcoming.Domain;
using RcCloud.DateScraper.Domain.Races;
using RcCloud.DateScraper.Domain.Regions;
using RcCloud.DateScraper.Domain.Series;

namespace RcCloud.DateScraper.Application.Myrcm.Upcoming.Services.V2;

public class ScrapeMyrcmRacesV2(
    IDownloadMyrcmPages downloadPages,
    IGuessClub guessClub,
    IGuessSeriesFromTitle guessSeriesFromTitle,
    IGuessIfItIsTraining guessIfItIsTraining,
    ILogger<ScrapeMyrcmRacesV2> logger) : IScrapeMyrcmRaces
{
    private static readonly Regex NumericDate = new(
        @"(?<!\d)(?:\d{1,2}\.\d{1,2}\.\d{4}|\d{4}-\d{1,2}-\d{1,2})(?!\d)",
        RegexOptions.Compiled);

    private static readonly Regex TextDate = new(
        @"(?<!\d)\d{1,2}\s+[A-Za-zÄÖÜäöü]+\s+\d{4}(?!\d)",
        RegexOptions.Compiled);

    public async Task<List<RaceMeeting>> Scrape(MyrcmCountryCode[] countries)
    {
        var content = await downloadPages.Download(new DownloadFilter(countries));
        return (await Scrape(content, countries)).ToList();
    }

    public async Task<IEnumerable<RaceMeeting>> Scrape(string content, MyrcmCountryCode[] countries)
    {
        var document = await BrowsingContext.New(Configuration.Default).OpenAsync(request => request.Content(content));
        var meetings = new List<RaceMeeting>();

        foreach (var table in document.QuerySelectorAll("table"))
        {
            var rows = table.QuerySelectorAll("tr");
            if (rows.Length < 2)
            {
                continue;
            }

            var headerRow = rows.FirstOrDefault(row => row.QuerySelectorAll("th").Length > 0);
            var headers = headerRow?.QuerySelectorAll("th").Select(cell => cell.TextContent.Trim()).ToArray()
                ?? rows[0].QuerySelectorAll("td").Select(cell => cell.TextContent.Trim()).ToArray();
            var dataRows = rows.Where(row => row != headerRow).ToArray();
            var dateColumn = FindColumn(headers, "date", "datum", "termin");
            var titleColumn = FindColumn(headers, "event", "race", "rennen", "anlass", "title");
            var clubColumn = FindColumn(headers, "club", "organizer", "organisator", "veranstalter");
            var countryColumn = FindColumn(headers, "country", "land");

            foreach (var row in dataRows)
            {
                var cells = row.QuerySelectorAll("td");
                if (cells.Length == 0)
                {
                    continue;
                }

                var dateCell = GetCell(cells, dateColumn) ?? cells.FirstOrDefault(cell => HasDate(cell.TextContent));
                var dates = ParseDates(dateCell?.TextContent ?? row.TextContent);
                if (dates.Length == 0)
                {
                    continue;
                }

                var title = GetCell(cells, titleColumn)?.TextContent.Trim();
                title = string.IsNullOrWhiteSpace(title) ? GetEventTitle(row, dateCell) : title;
                if (string.IsNullOrWhiteSpace(title) || guessIfItIsTraining.IsTraining(title))
                {
                    continue;
                }

                var country = ParseCountry(GetCellValue(GetCell(cells, countryColumn)) ?? row.TextContent);
                if (country is null || !countries.Any(candidate => candidate.Code == country.Code))
                {
                    continue;
                }

                var clubCell = GetCell(cells, clubColumn) ?? FindClubCell(cells, dateCell, title);
                var clubName = clubCell?.TextContent.Trim();
                if (string.IsNullOrWhiteSpace(clubName))
                {
                    continue;
                }

                var countryCode = GetCountryCode(country);
                var clubNumber = GetClubNumber(clubCell);
                var race = new MyrcmRace(
                    dates[0],
                    dates[^1],
                    country,
                    title,
                    clubName,
                    clubNumber);
                var club = guessClub.Guess(race.Club, clubNumber, countryCode);
                var regions = new List<RegionReference>();
                if (club.Region is not null)
                {
                    regions.Add(club.Region);
                }

                regions.Add(new RegionReference(countryCode));

                meetings.Add(new RaceMeeting(
                    guessSeriesFromTitle.Guess(race.Title),
                    SeasonReference.Current,
                    race.DateEnd,
                    club.Name,
                    countryCode,
                    race.Title,
                    regions.ToArray(),
                    club,
                    "Myrcm"));
            }
        }

        if (meetings.Count == 0)
        {
            logger.LogWarning("Myrcm V2 scraping found no upcoming races for the requested countries.");
        }

        return meetings;
    }

    private static int FindColumn(string[] headers, params string[] names)
    {
        for (var index = 0; index < headers.Length; index++)
        {
            var header = headers[index].ToLowerInvariant();
            if (names.Any(name => header.Contains(name, StringComparison.OrdinalIgnoreCase)))
            {
                return index;
            }
        }

        return -1;
    }

    private static IElement? GetCell(IHtmlCollection<IElement> cells, int index)
    {
        return index >= 0 && index < cells.Length ? cells[index] : null;
    }

    private static string? GetCellValue(IElement? cell)
    {
        if (cell is null)
        {
            return null;
        }

        var text = cell.TextContent.Trim();
        if (text.Length > 0)
        {
            return text;
        }

        return string.Join(" ", cell.QuerySelectorAll("[alt], [title]")
            .Select(element => element.GetAttribute("alt") ?? element.GetAttribute("title"))
            .Where(value => !string.IsNullOrWhiteSpace(value)));
    }

    private static IElement? FindClubCell(IHtmlCollection<IElement> cells, IElement? dateCell, string title)
    {
        return cells.FirstOrDefault(cell =>
            cell != dateCell &&
            !string.Equals(cell.TextContent.Trim(), title, StringComparison.OrdinalIgnoreCase) &&
            !HasDate(cell.TextContent));
    }

    private static string GetEventTitle(IElement row, IElement? dateCell)
    {
        var links = row.QuerySelectorAll("a")
            .Where(link => link.TextContent.Trim().Length > 0)
            .ToArray();

        var eventLink = links.FirstOrDefault(link => !ContainsDate(link.TextContent));
        if (eventLink is not null)
        {
            return eventLink.TextContent.Trim();
        }

        return row.QuerySelectorAll("td")
            .FirstOrDefault(cell => cell != dateCell && !HasDate(cell.TextContent))
            ?.TextContent.Trim() ?? string.Empty;
    }

    private static bool HasDate(string text) => NumericDate.IsMatch(text) || TextDate.IsMatch(text);

    private static bool ContainsDate(string text) => HasDate(text);

    private static DateOnly[] ParseDates(string text)
    {
        var candidates = NumericDate.Matches(text)
            .Select(match => (Position: match.Index, Value: match.Value))
            .Concat(TextDate.Matches(text).Select(match => (Position: match.Index, Value: match.Value)))
            .OrderBy(match => match.Position);

        return candidates
            .Select(match => ParseDate(match.Value))
            .Where(date => date.HasValue)
            .Select(date => date!.Value)
            .ToArray();
    }

    private static DateOnly? ParseDate(string text)
    {
        string[] formats = ["d.M.yyyy", "dd.MM.yyyy", "yyyy-M-d", "yyyy-MM-dd", "d MMMM yyyy", "dd MMMM yyyy", "d MMM yyyy", "dd MMM yyyy"];
        var cultures = new[] { CultureInfo.GetCultureInfo("de-DE"), CultureInfo.GetCultureInfo("en-GB") };
        foreach (var culture in cultures)
        {
            if (DateOnly.TryParseExact(text, formats, culture, DateTimeStyles.AllowWhiteSpaces, out var date))
            {
                return date;
            }
        }

        return null;
    }

    private static MyrcmCountryCode? ParseCountry(string text)
    {
        var country = text.ToLowerInvariant();
        if (country.Contains("deu") || country.Contains("germany") || country.Contains("deutschland") || country.Trim() == "de")
        {
            return MyrcmCountryCode.Germany;
        }

        if (country.Contains("nld") || country.Contains("netherlands") || country.Contains("nederland") || country.Contains("niederlande") || country.Trim() == "nl")
        {
            return MyrcmCountryCode.Netherlands;
        }

        if (country.Contains("bel") || country.Contains("belgium") || country.Contains("belgien") || country.Contains("belgië") || country.Trim() == "be")
        {
            return MyrcmCountryCode.Belgium;
        }

        if (country.Contains("lux") || country.Contains("luxembourg") || country.Contains("luxemburg") || country.Trim() == "lu")
        {
            return MyrcmCountryCode.Luxembourg;
        }

        return null;
    }

    private static string GetCountryCode(MyrcmCountryCode country)
    {
        return country.Code switch
        {
            3 => "de",
            5 => "lux",
            8 => "be",
            9 => "nl",
            _ => string.Empty,
        };
    }

    private static int GetClubNumber(IElement? clubCell)
    {
        var href = clubCell?.QuerySelector("a")?.GetAttribute("href");
        if (string.IsNullOrWhiteSpace(href))
        {
            return 0;
        }

        var decodedHref = Uri.UnescapeDataString(href);
        var match = Regex.Match(decodedHref, @"[?&]dId\[O\]=(\d+)", RegexOptions.IgnoreCase);
        if (match.Success && int.TryParse(match.Groups[1].Value, out var decodedClubNumber))
        {
            return decodedClubNumber;
        }

        var absoluteUrl = Uri.TryCreate(href, UriKind.Absolute, out var absolute)
            ? absolute
            : new Uri(new Uri(DownloadMyrcmPagesV2.UpcomingUrl), href);
        var query = Uri.UnescapeDataString(absoluteUrl.Query);
        var clubNumber = HttpUtility.ParseQueryString(query).Get("dId[O]");

        return int.TryParse(clubNumber, out var number) ? number : 0;
    }
}
