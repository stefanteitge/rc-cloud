using System.Diagnostics;
using System.Globalization;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using RcCloud.DateScraper.Application.Rcco.Domain;
using RcCloud.DateScraper.Domain.Races;

namespace RcCloud.DateScraper.Application.Rcco.Services;

public class ScrapeRcco(IGuessClub guessClub, ILogger<ScrapeRcco> logger)
{
    public const string BaseUrl = "https://rccar-online.de/veranstaltungen";

    public const string UserAgent =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/130.0.0.0 Safari/537.36";

    private static readonly HttpClient Client = CreateClient();

    /// <summary>
    /// Separate connect timeout so the logs tell a blocked connection apart from a slow response.
    /// </summary>
    public static HttpClient CreateClient()
    {
        var handler = new SocketsHttpHandler { ConnectTimeout = TimeSpan.FromSeconds(10) };
        var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(15) };
        client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
        client.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
        client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("de-DE,de;q=0.9,en;q=0.8");
        return client;
    }

    public async Task<List<RaceMeeting>> Scrape()
    {
        var stopwatch = Stopwatch.StartNew();
        using var res = await Client.GetAsync(BaseUrl);
        var html = await res.Content.ReadAsStringAsync();

        logger.LogInformation(
            "RCCO scraping: GET {Url} returned HTTP {StatusCode} with {Length} chars in {ElapsedMs}ms.",
            BaseUrl,
            (int)res.StatusCode,
            html.Length,
            stopwatch.ElapsedMilliseconds);

        if (!res.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"RCCO returned HTTP {(int)res.StatusCode} {res.ReasonPhrase}. Body starts with: {Preview(html)}",
                null,
                res.StatusCode);
        }

        var races = Scrape(html);
        logger.LogInformation("RCCO scraping: parsed {Count} races.", races.Count);

        return races;
    }

    public List<RaceMeeting> Scrape(string content)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(content);
        
        var allTables = doc.DocumentNode.SelectNodes("//table")
            ?? throw new InvalidOperationException($"RCCO page contains no tables. Content starts with: {Preview(content)}");
        var tables = allTables.Skip(2);
            
        var events = new List<RccoVeranstaltung>();
        foreach (var table in tables)
        {
            events.AddRange(ParseTable(table));    
        }

        List<RaceMeeting> raceMeetings = new List<RaceMeeting>();
        foreach (var evt in events)
        {
            var club = guessClub.Guess(evt.Verein, "de");
            raceMeetings.Add(evt.ToRaceMeeting(club));
        }
        
        return raceMeetings;
    }

    private List<RccoVeranstaltung> ParseTable(HtmlNode table)
    {
        var rows = table.SelectNodes("tr").Skip(1);

        List<RccoVeranstaltung> veranstaltungen = new List<RccoVeranstaltung>();
        foreach (var row in rows)
        {
            var cells = row.SelectNodes("td");
            if (cells != null)
            {
                var dateEnd = cells[0].ChildNodes
                    .Last(n => n.NodeType == HtmlNodeType.Text && !string.IsNullOrEmpty(n.InnerText.Trim())).InnerText;
                var laufname = cells[2].ChildNodes.FirstOrDefault()?.InnerText.Trim() ?? "Rennen";

                var verein = cells[1].SelectSingleNode("a").InnerText;
                var strecke = cells[8].SelectSingleNode("a")?.InnerText;

                var veranstaltung = new RccoVeranstaltung(ParseDate(dateEnd), verein, laufname, strecke ?? "Unbekannt");

                veranstaltungen.Add(veranstaltung);
            }
        }

        return veranstaltungen;
    }
    
    private DateOnly ParseDate(string input)
    {
        return DateOnly.ParseExact(input, "dd.MM.yyyy", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
    }

    private static string Preview(string content)
    {
        var flat = string.Join(' ', content.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
        return flat.Length <= 300 ? flat : flat[..300] + "…";
    }
}
