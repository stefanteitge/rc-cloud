using RcCloud.DateScraper.Domain;

namespace RcCloud.DateScraper.Cli.Commands;

internal class RaceMeetingPrinter
{
    public void Print(IEnumerable<RaceMeeting> raceMeetings)
    {
        foreach (var raceMeeting in raceMeetings)
        {
            Print(raceMeeting);
        }
    }
    public void Print(RaceMeeting raceMeeting)
    {
        Console.Write(raceMeeting.Date);
        Console.Write("\t");
        Console.Write(string.Join(", ", raceMeeting.Series.Select(s => s.Id)));
        Console.Write("\t ");
        Console.Write(raceMeeting.Title);
        Console.Write("\t ");
        Console.Write(raceMeeting.Location);
        Console.Write("\t(");
        Console.Write(string.Join(", ", raceMeeting.Groups.Select(g => g.Id)));
        Console.WriteLine(")");
    }
}
