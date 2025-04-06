using System.Globalization;
using HtmlAgilityPack;
using RcCloud.DateScraper.Domain.Races;

namespace RcCloud.DateScraper.Application.Rcco;

public class ScrapeRcco
{
    private const string BaseUrl = "https://rccar-online.de/veranstaltungen";

    public async Task<List<RaceMeeting>> Scrape()
    {
        HttpClient client = new HttpClient();

        var res= await client.GetAsync(BaseUrl);

        string html = await res.Content.ReadAsStringAsync();
        
        return await Parse(html);
    }

    public async Task<List<RaceMeeting>> Parse(string content)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(content);
        
        var tables = doc.DocumentNode.SelectNodes("//table").Skip(2);
            
        var events = new List<RccoRace>();
        foreach (var table in tables)
        {
            events.AddRange(ParseTable(table));    
        }

        return events.Select(e => e.ToRaceMeeting()).ToList();
    }

    private List<RccoRace> ParseTable(HtmlNode table)
    {
        var rows = table.SelectNodes("tr").Skip(1);

        List<RccoRace> events = new List<RccoRace>();
        foreach (var row in rows)
        {
            var cells = row.SelectNodes("td");
            if (cells != null)
            {
                var dateEnd = cells[0].ChildNodes
                    .Last(n => n.NodeType == HtmlNodeType.Text && !string.IsNullOrEmpty(n.InnerText.Trim())).InnerText;
                var title = cells[2].ChildNodes.FirstOrDefault()?.InnerText.Trim() ?? "Rennen";
                
                var evt = new RccoRace(title)
                {
                    DateStart = cells[0].ChildNodes.First(n => n.NodeType == HtmlNodeType.Text && !string.IsNullOrEmpty(n.InnerText.Trim())).InnerText,
                    DateEnd = ParseDate(dateEnd),
                    Club = cells[1].SelectSingleNode("a").InnerText,
                    Location = cells[8].SelectSingleNode("a").InnerText,
                };

                events.Add(evt);
            }
        }

        return events;
    }
    
    private DateOnly ParseDate(string input)
    {
        return DateOnly.ParseExact(input, "dd.MM.yyyy", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
    }
}
