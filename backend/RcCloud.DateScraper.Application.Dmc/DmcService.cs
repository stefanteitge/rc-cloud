using HtmlAgilityPack;
using RcCloud.DateScraper.Domain;
using System.Globalization;

namespace RcCloud.DateScraper.Application.Dmc;

public class DmcService
{
    private const string BaseUrl = "https://dmc-online.com/wordpress/termine/dmc-termine/";

    public async Task<IEnumerable<RaceMeeting>> Parse()
    {
        var client = new HttpClient();
        var all = await Scrape(2025, client);

        return all
            .Where(ce => ce.IsMeeting())
            .Select(a => new RaceMeeting(new("dmc"), SeasonReference.Current, a.DateEnd, a.Club, ComputeRegions(a)));
    }

    private GroupReference[] ComputeRegions(DmcCalendarEntry a)
    {
        if (a.IsRegionMeeting(DmcRegion.Central))
        {
            return [GroupReference.Central];
        }

        if (a.IsRegionMeeting(DmcRegion.North))
        {
            return [GroupReference.North];
        }

        if (a.IsRegionMeeting(DmcRegion.West))
        {
            return [GroupReference.West];
        }

        if (a.IsRegionMeeting(DmcRegion.South))
        {
            return [GroupReference.South];
        }

        if (a.IsRegionMeeting(DmcRegion.East))
        {
            return [GroupReference.East];
        }

        return [];
    }

    public async Task<string> RetrieveBaseDocument(int year, HttpClient httpClient)
    {
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("startmonat", "01"),
            new KeyValuePair<string, string>("endmonat", "01"),
            new KeyValuePair<string, string>("jahr", $"{year}"),
            new KeyValuePair<string, string>("praedikat", "null"),
            new KeyValuePair<string, string>("klasse[]", "Alle Startklassen"),
            new KeyValuePair<string, string>("submit", "Termine anzeigen"),
        });
        
        var res = await httpClient.PostAsync(BaseUrl, content);
        return await res.Content.ReadAsStringAsync();
    }

    public async Task<List<DmcCalendarEntry>> Scrape(int year, HttpClient httpClient)
    {
        var html = await RetrieveBaseDocument(year, httpClient);
        return ScrapeRaw(html);
    }

    public List<DmcCalendarEntry> ScrapeRaw(string rawInput)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(rawInput);
        
        var table = doc.DocumentNode.SelectSingleNode("//table");
        var rows = table.SelectNodes("tr");

        List<DmcCalendarEntry> events = new List<DmcCalendarEntry>();
        foreach (var row in rows)
        {
            var cells = row.SelectNodes("td");
            if (cells != null)
            {
                var evt = new DmcCalendarEntry()
                {
                    DateStart = ParseDate(cells[0].InnerText),
                    DateEnd = ParseDate(cells[1].InnerText),
                    Type = cells[2].InnerText,
                    Classes = cells[3].ChildNodes.Where(n => n.NodeType == HtmlNodeType.Text).Select(n => n.InnerText).ToArray(),
                    ClubNo = ParseClubNo(cells[4].InnerText),
                    Club = cells[5].InnerText,
                    Location = cells[6].InnerText,
                    Comment = cells[7].InnerText,
                    Announcement = cells[8].ChildNodes.Where(n => n.Name != "br").Select(n => n.OuterHtml).ToArray(),
                    Entering = cells[9].ChildNodes.Where(n => n.Name != "br").Select(n => n.OuterHtml).ToArray(),
                    Results = cells[10].ChildNodes.Where(n => n.Name != "br").Select(n => n.OuterHtml).ToArray(),
                    Related = cells[11].ChildNodes.Where(n => n.Name != "br").Select(n => n.OuterHtml).ToArray(),
                };

                events.Add(evt);
                //queue.Add(evt);
            }
        }
        return events;
    }

    private int? ParseClubNo(string innerText)
    {
        var success = int.TryParse(innerText, out var result);

        if (success)
        {
            return result;
        }

        return null;
    }

    private DateOnly ParseDate(string input)
    {
        return DateOnly.ParseExact(input, "dd.MM.yyyy", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
    }
}
