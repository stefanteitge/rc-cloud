using HtmlAgilityPack;

namespace RcCloud.DateScraper.Application.Rcco;

public class RccoService
{
    private const string BaseUrl = "https://rccar-online.de/veranstaltungen";
        
    public async Task<List<RccoEvent>> RunAsync()
    {
        HttpClient client = new HttpClient();

        var res= await client.GetAsync(BaseUrl);

        string html = await res.Content.ReadAsStringAsync();
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        
        var tables = doc.DocumentNode.SelectNodes("//table").Skip(2);
            
        var events = new List<RccoEvent>();
        foreach (var table in tables)
        {
            events.AddRange(ParseTable(table));    
        }
            
        return events;
    }

    private static List<RccoEvent> ParseTable(HtmlNode table)
    {
        var rows = table.SelectNodes("tr").Skip(1);

        List<RccoEvent> events = new List<RccoEvent>();
        foreach (var row in rows)
        {
            var cells = row.SelectNodes("td");
            if (cells != null)
            {
                var evt = new RccoEvent()
                {
                    DateStart = cells[0].ChildNodes.First(n => n.NodeType == HtmlNodeType.Text && !string.IsNullOrEmpty(n.InnerText.Trim())).InnerText,
                    DateEnd = cells[0].ChildNodes.Last(n => n.NodeType == HtmlNodeType.Text && !string.IsNullOrEmpty(n.InnerText.Trim())).InnerText,
                    Club = cells[1].SelectSingleNode("a").InnerText,
                    Location = cells[8].SelectSingleNode("a").InnerText,
                };

                events.Add(evt);
            }
        }

        return events;
    }
}