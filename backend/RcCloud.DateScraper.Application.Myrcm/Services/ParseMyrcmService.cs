using System.Globalization;
using System.Runtime.InteropServices.JavaScript;
using AngleSharp;
using AngleSharp.Dom;
using RcCloud.DateScraper.Domain;

namespace RcCloud.DateScraper.Application.Myrcm.Services;

public class ParseMyrcmService(DownloadMyrcmPageService downloadPageService)
{
    public async Task<IEnumerable<RaceMeeting>> Parse()
    {
        var downloaded = await downloadPageService.Download(DownloadFilter.GermanyOnly);
        return await Parse(downloaded);
    }
    
    public async Task<IEnumerable<RaceMeeting>> Parse(string content)
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

            var date = ParseDate(cells[4].TextContent);
            var location = cells[1]?.TextContent;
            var title = cells[2]?.TextContent ?? "MyRCM-Rennen";
            var meeting = new RaceMeeting(CompileSeries(title), SeasonReference.Current, date, location,
                title, []);
            
            meetings.Add(meeting);
        }

        return meetings;
    }

    private static SeriesReference[] CompileSeries(string title)
    {
        var seriess = new List<SeriesReference>();

        if (title.Contains("Hudy Series", StringComparison.InvariantCultureIgnoreCase))
        {
            seriess.Add(SeriesReference.Hudy);
        }
        
        if (title.Contains("Tamiya Euro", StringComparison.InvariantCultureIgnoreCase))
        {
            seriess.Add(SeriesReference.Tamiya);
        }
        
        if (title.Contains("ostmasters", StringComparison.InvariantCultureIgnoreCase))
        {
            seriess.Add(new SeriesReference("ostmasters"));
        }
        
        if (title.Contains("jumpstart", StringComparison.InvariantCultureIgnoreCase))
        {
            seriess.Add(new SeriesReference("jumpstart"));
        }
        
        if (title.Contains("ElbeCup", StringComparison.InvariantCultureIgnoreCase))
        {
            seriess.Add(new SeriesReference("elbecup"));
        }
        
        if (seriess.Count > 0)
        {
            return seriess.ToArray();
        }
        
        return [new SeriesReference("myrcm")];
    }

    private DateOnly ParseDate(string input)
    {
        return DateOnly.ParseExact(input, "dd.MM.yyyy", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
    }
}