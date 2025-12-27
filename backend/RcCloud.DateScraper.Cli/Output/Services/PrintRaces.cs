using RcCloud.DateScraper.Domain.Races;

namespace RcCloud.DateScraper.Cli.Output.Services;

internal class PrintRaces
{
    public void Print(IEnumerable<RaceMeeting> raceMeetings)
    {
        var meetings = raceMeetings.ToList();
        if (!meetings.Any()) return;

        // Calculate max widths for each column
        int dateWidth = meetings.Max(r => r.Date.ToString().Length);
        int seriesWidth = meetings.Max(r => string.Join(", ", r.Series.Select(s => s.Id)).Length);
        int titleWidth = 50;
        int locationWidth = meetings.Max(r => r.Location.Length);
        int regionsWidth = meetings.Max(r => string.Join(", ", r.Regions.Select(g => g.Id)).Length);

        foreach (var raceMeeting in meetings)
        {
            Print(raceMeeting, dateWidth, seriesWidth, titleWidth, locationWidth, regionsWidth);
        }
    }

    private void Print(RaceMeeting raceMeeting, int dateWidth, int seriesWidth, int titleWidth, int locationWidth, int regionsWidth)
    {
        Console.Write(raceMeeting.Date.ToString().PadRight(dateWidth));
        Console.Write(" ");
        Console.Write(string.Join(", ", raceMeeting.Series.Select(s => s.Id)).PadRight(seriesWidth));
        Console.Write(" ");
        // Trim title to 50 characters for display
        string trimmedTitle = raceMeeting.Title.Length > 50 ? raceMeeting.Title.Substring(0, 50) : raceMeeting.Title;
        Console.Write(trimmedTitle.PadRight(titleWidth));
        Console.Write(" ");
        Console.Write(raceMeeting.Location.PadRight(locationWidth));
        Console.Write(" ");
        Console.WriteLine(string.Join(", ", raceMeeting.Regions.Select(g => g.Id)));
    }
}
