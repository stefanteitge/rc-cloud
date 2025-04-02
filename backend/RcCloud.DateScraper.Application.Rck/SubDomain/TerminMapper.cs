using RcCloud.DateScraper.Domain;
using RcCloud.DateScraper.Domain.Races;

namespace RcCloud.DateScraper.Application.Rck.SubDomain
{
    internal static class TerminMapper
    {
        public static RaceMeeting ToDomain(this DatedEvent datedEvent, SeriesReference series)
        {
            return new RaceMeeting(
                [series],
                SeasonReference.Current,
                datedEvent.Date,
                datedEvent.Location,
                GetTitle(series),
                datedEvent.Gruppen.Select(g => ToReference(g)).ToArray());
        }

        private static string GetTitle(SeriesReference series)
        {
            if (series.Id == "kleinserie")
            {
                return "Kleinserie";
            }
            
            if (series.Id == "challenge")
            {
                return "RCK-Challenge";
            }

            return "Unbekannt";
        }

        private static GroupReference ToReference(Gruppe g)
        {
            return g switch
            {
                Gruppe.Mitte => GroupReference.Central,
                Gruppe.Nord => GroupReference.North,
                Gruppe.West => GroupReference.West,
                Gruppe.Sued => GroupReference.South,
                Gruppe.Ost => GroupReference.East,
                _ => throw new ArgumentOutOfRangeException(nameof(g), g, null)
            };
        }
    }
}
