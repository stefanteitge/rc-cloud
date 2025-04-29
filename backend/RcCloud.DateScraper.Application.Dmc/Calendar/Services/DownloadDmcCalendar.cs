﻿using System.Globalization;
using FluentResults;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using RcCloud.DateScraper.Application.Dmc.Calendar.Domain;

namespace RcCloud.DateScraper.Application.Dmc.Calendar.Services;

public class DownloadDmcCalendar(ILogger<DownloadDmcCalendar> logger)
{
    private const string BaseUrl = "https://dmc-online.com/wordpress/termine/dmc-termine/";
    
    public async Task<Result<List<DmcCalendarEntry>>> Scrape(int year)
    {
        var downloadResult = await RetrieveBaseDocument(year);

        if (downloadResult.IsFailed)
        {
            return Result.Fail(downloadResult.Errors);
        }

        return ScrapeRaw(downloadResult.Value);
    }
    
    public async Task<Result<string>> RetrieveBaseDocument(int year)
    {
        var message = new HttpRequestMessage(HttpMethod.Post, BaseUrl);
        message.Headers.Referrer = new Uri("https://dmc-online.com/wordpress/termine/dmc-termine/");
        
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("startmonat", "01"),
            new KeyValuePair<string, string>("endmonat", "01"),
            new KeyValuePair<string, string>("jahr", $"{year}"),
            new KeyValuePair<string, string>("praedikat", "null"),
            new KeyValuePair<string, string>("klasse[]", "Alle Startklassen"),
            new KeyValuePair<string, string>("submit", "Termine anzeigen"),
        });
        
        message.Content = content;
        
        var client = new HttpClient();
        var res = await client.SendAsync(message);

        if (!res.IsSuccessStatusCode)
        {
            var limit = res.Headers.GetValues("X-Ws-Ratelimit-Remaining").FirstOrDefault();

            if (limit is not null)
            {
                logger.LogInformation($"RateLimit in reponse header at {limit}");
            }
            
            return new DownloadError(BaseUrl, $"DMC scraping: Received a HTTP {res.StatusCode} on document retrieval");
        }

        return await res.Content.ReadAsStringAsync();
    }
    
    public Result<List<DmcCalendarEntry>> ScrapeRaw(string rawInput)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(rawInput);

        logger.LogInformation("DMC scraping: Loaded HTML document with size {InputSize}", rawInput.Length);

        var table = doc.DocumentNode.SelectSingleNode("//table");

        if (table is null)
        {
            var error = new ScrapeError("DMC scraping aborted: no table found in source");
            logger.LogError(error.Message);
            return error;
        }

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

            var ausschreibung = cells[8].ChildNodes.Where(n => n.Name != "br").Select(n => n.OuterHtml).ToArray();
            var nennung = cells[9].ChildNodes.Where(n => n.Name != "br").Select(n => n.OuterHtml).ToArray();
            var ergebnis = cells[10].ChildNodes.Where(n => n.Name != "br").Select(n => n.OuterHtml).ToArray();
            var zusatzinfos = cells[11].ChildNodes.Where(n => n.Name != "br").Select(n => n.OuterHtml).ToArray();
            var verein = cells[5].InnerText.Trim();

            // drop international races and meetings
            if (verein.StartsWith("Referent") || verein.StartsWith("Sportkreisvorsitzender"))
            {
                continue;
            }
            
            var evt = new DmcCalendarEntry(
                ParseDate(cells[0].InnerText),
                ParseDate(cells[1].InnerText),
                cells[2].InnerText,
                cells[3].ChildNodes.Where(n => n.NodeType == HtmlNodeType.Text).Select(n => n.InnerText).ToArray(),
                ParseClubNo(cells[4].InnerText),
                verein,
                cells[6].InnerText,
                cells[7].InnerText,
                ausschreibung,
                nennung,
                ergebnis,
                zusatzinfos);

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
