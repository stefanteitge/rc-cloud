using System.Text.RegularExpressions;
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
    // Safety net in case the paging info stops advancing; the live site has ~20 pages of 25 events.
    private const int MaxPages = 200;

    private static readonly Regex OrganizerId = new(@"/organizers/(\d+)", RegexOptions.Compiled);

    private static readonly Dictionary<string, int> Months = new(StringComparer.OrdinalIgnoreCase)
    {
        ["jan"] = 1, ["feb"] = 2, ["mär"] = 3, ["mrz"] = 3, ["mar"] = 3, ["apr"] = 4,
        ["mai"] = 5, ["may"] = 5, ["jun"] = 6, ["jul"] = 7, ["aug"] = 8, ["sep"] = 9,
        ["okt"] = 10, ["oct"] = 10, ["nov"] = 11, ["dez"] = 12, ["dec"] = 12,
    };

    private static readonly Dictionary<string, MyrcmCountryCode> Countries = new(StringComparer.OrdinalIgnoreCase)
    {
        ["DEU"] = MyrcmCountryCode.Germany,
        ["NLD"] = MyrcmCountryCode.Netherlands,
        ["BEL"] = MyrcmCountryCode.Belgium,
        ["LUX"] = MyrcmCountryCode.Luxembourg,
    };

    public async Task<List<RaceMeeting>> Scrape(MyrcmCountryCode[] countries)
    {
        var filter = new DownloadFilter(countries);
        var meetings = new List<RaceMeeting>();

        for (var pageIndex = 0; pageIndex < MaxPages; pageIndex++)
        {
            var page = MyrcmUpcomingPage.Parse(await downloadPages.Download(filter, pageIndex));
            var cards = await ParseCards(page.Html);
            meetings.AddRange(ToMeetings(cards, countries));

            if (page.IsLastPage || cards.Count == 0)
            {
                break;
            }
        }

        if (meetings.Count == 0)
        {
            logger.LogWarning("Myrcm V2 scraping found no upcoming races for the requested countries.");
        }

        return meetings;
    }

    /// <summary>
    /// Scrapes a single JSON page as returned by <see cref="DownloadMyrcmPagesV2"/>.
    /// </summary>
    public async Task<IEnumerable<RaceMeeting>> Scrape(string json, MyrcmCountryCode[] countries)
    {
        var page = MyrcmUpcomingPage.Parse(json);
        return ToMeetings(await ParseCards(page.Html), countries);
    }

    internal static async Task<List<MyrcmRace>> ParseCards(string html)
    {
        var document = await BrowsingContext.New(Configuration.Default).OpenAsync(request => request.Content(html));
        var races = new List<MyrcmRace>();

        foreach (var card in document.QuerySelectorAll("article.event-card"))
        {
            var dateStart = ParseDateBox(card.QuerySelector(".date-box.from"));
            if (dateStart is null)
            {
                continue;
            }

            var dateEnd = ParseDateBox(card.QuerySelector(".date-box.to")) ?? dateStart.Value;
            var title = card.QuerySelector(".event-title")?.TextContent.Trim() ?? string.Empty;
            var organizer = card.QuerySelector(".event-meta a[href*='/organizers/']");
            var clubName = organizer?.TextContent.Trim() ?? string.Empty;
            var countryCode = card.QuerySelector(".badge.country")?.TextContent.Trim() ?? string.Empty;

            races.Add(new MyrcmRace(
                dateStart.Value,
                dateEnd,
                Countries.GetValueOrDefault(countryCode),
                title,
                clubName,
                GetClubNumber(organizer)));
        }

        return races;
    }

    private List<RaceMeeting> ToMeetings(IEnumerable<MyrcmRace> races, MyrcmCountryCode[] countries)
    {
        var meetings = new List<RaceMeeting>();

        foreach (var race in races)
        {
            if (race.Country is null || !countries.Any(candidate => candidate.Code == race.Country.Code))
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(race.Title) || string.IsNullOrWhiteSpace(race.Club) || guessIfItIsTraining.IsTraining(race.Title))
            {
                continue;
            }

            var countryCode = GetCountryCode(race.Country);
            var club = guessClub.Guess(race.Club, race.ClubNumber ?? 0, countryCode);
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

        return meetings;
    }

    private static DateOnly? ParseDateBox(IElement? dateBox)
    {
        if (dateBox is null)
        {
            return null;
        }

        var dayText = dateBox.QuerySelector(".date-day")?.TextContent.Trim();
        var monthText = dateBox.QuerySelector(".date-month")?.TextContent.Trim();
        var yearText = dateBox.QuerySelector(".date-year")?.TextContent.Trim();

        if (!int.TryParse(dayText, out var day)
            || !int.TryParse(yearText, out var year)
            || monthText is null
            || monthText.Length < 3
            || !Months.TryGetValue(monthText[..3], out var month))
        {
            return null;
        }

        try
        {
            return new DateOnly(year, month, day);
        }
        catch (ArgumentOutOfRangeException)
        {
            return null;
        }
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

    private static int GetClubNumber(IElement? organizer)
    {
        var href = organizer?.GetAttribute("href");
        if (string.IsNullOrWhiteSpace(href))
        {
            return 0;
        }

        var match = OrganizerId.Match(href);
        return match.Success && int.TryParse(match.Groups[1].Value, out var number) ? number : 0;
    }
}
