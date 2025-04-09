using System.Globalization;
using AngleSharp;
using AngleSharp.Dom;
using RcCloud.DateScraper.Application.Rck.SubDomain;
using RcCloud.DateScraper.Domain.Races;
using RcCloud.DateScraper.Domain.Series;

namespace RcCloud.DateScraper.Application.Rck.Services;

public abstract class AbstractRckService
{
    protected async Task<List<RaceMeeting>> Parse(SeriesReference series, Url baseUrl, string source)
    {
        var document = await BrowsingContext.New(Configuration.Default.WithDefaultLoader()).OpenAsync(baseUrl, CancellationToken.None);

        if (document is null)
        {
            return [];
        }

        return Parse2(document, series, source);
    }

    protected async Task<List<RaceMeeting>> Scrape(SeriesReference series, string content, string source)
    {
        var document = await BrowsingContext.New(Configuration.Default.WithDefaultLoader()).OpenAsync(req => req.Content(content));

        if (document is null)
        {
            return [];
        }

        return Parse2(document, series, source);
    }

    private List<RaceMeeting> Parse2(IDocument document, SeriesReference series, string source)
    {
        var dateTable = document.QuerySelector("body > table:nth-child(5)");

        if (dateTable is null)
        {
            return [];
        }

        var rows = dateTable.QuerySelectorAll("tr").Skip(2);

        var all = new List<Renntermin>();
        foreach (var row in rows)
        {
            var date = ParseDate(row.QuerySelector("td"));
            if (date is null)
            {
                continue;
            }

            IEnumerable<UndatierterTermin?> events = [
                ParseCell(row.QuerySelector("td:nth-child(2)"), Gruppe.Mitte),
                ParseCell(row.QuerySelector("td:nth-child(3)"), Gruppe.Nord),
                ParseCell(row.QuerySelector("td:nth-child(4)"), Gruppe.West),
                ParseCell(row.QuerySelector("td:nth-child(5)"), Gruppe.Sued),
                ParseCell(row.QuerySelector("td:nth-child(6)"), Gruppe.Ost),
            ];

            var grouped = events
                .OfType<UndatierterTermin>()
                .GroupBy(e => e.Location)
                .Select(es => new Renntermin(date.Value, es, source));

            all.AddRange(grouped);
        }

        return all.Select(r => r.ToDomain(series)).ToList();
    }

    private DateOnly? ParseDate(IElement? element)
    {
        if (element is null)
        {
            return null;
        }

        var date = element.TextContent;

        if (date.Contains(" - ")) {
            date = date.Split(" - ")[1];
        }

        var parsed = DateOnly.ParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);

        return parsed;
    }

    private UndatierterTermin? ParseCell(IElement? element, Gruppe gruppe)
    {
        if (element is null)
        {
            return null;
        }

        var exists = element.QuerySelectorAll("a").Any();

        if (exists)
        {
            var location = element.QuerySelectorAll("b").Select(e => e.TextContent);
            var location2 = string.Join(" ", location).Trim();
            return new UndatierterTermin(location2, gruppe);
        }

        var isComingSoon = element.TextContent.Contains("coming soon");
        if (isComingSoon)
        {
            var location = element.QuerySelector("b font")?.TextContent.Replace("coming soon", "").Trim();
            return new UndatierterTermin(location ?? "Unbekannter Ort", gruppe);
        }

        return null;
    }
}
