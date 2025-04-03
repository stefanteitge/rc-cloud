using System.Globalization;
using HtmlAgilityPack;
using RcCloud.DateScraper.Application.Dmc.Calendar.Domain;

namespace RcCloud.DateScraper.Application.Dmc.Calendar.Services;

public class DownloadDmcCalendar
{
    private const string BaseUrl = "https://dmc-online.com/wordpress/termine/dmc-termine/";
    
    public async Task<List<DmcCalendarEntry>> Scrape(int year)
    {
        var html = await RetrieveBaseDocument(year);
        return ScrapeRaw(html);
    }
    
    public async Task<string> RetrieveBaseDocument(int year)
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
        
        var client = new HttpClient();
        var res = await client.PostAsync(BaseUrl, content);
        return await res.Content.ReadAsStringAsync();
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

            // required
            if (cells is null)
            {
                continue;
            }
            
            var evt = new DmcCalendarEntry(
                ParseDate(cells[0].InnerText),
                ParseDate(cells[1].InnerText),
                cells[2].InnerText,
                cells[3].ChildNodes.Where(n => n.NodeType == HtmlNodeType.Text).Select(n => n.InnerText).ToArray(),
                ParseClubNo(cells[4].InnerText),
                cells[5].InnerText,
                cells[6].InnerText)
            {
                Comment = cells[7].InnerText,
                Announcement = cells[8].ChildNodes.Where(n => n.Name != "br").Select(n => n.OuterHtml).ToArray(),
                Entering = cells[9].ChildNodes.Where(n => n.Name != "br").Select(n => n.OuterHtml).ToArray(),
                Results = cells[10].ChildNodes.Where(n => n.Name != "br").Select(n => n.OuterHtml).ToArray(),
                Related = cells[11].ChildNodes.Where(n => n.Name != "br").Select(n => n.OuterHtml).ToArray(),
            };

            events.Add(evt);
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