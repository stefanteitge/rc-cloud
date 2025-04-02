using RcCloud.DateScraper.Domain;

namespace RcCloud.DateScraper.Application.Rck.SubDomain
{
    internal static class TerminMapper
    {
        public static RaceMeeting ToDomain(this DatedEvent datedEvent, SeriesReference series)
        {
            return new RaceMeeting(
                series: series,
                season: SeasonReference.Current,
                date: datedEvent.Date,
                location: datedEvent.Location,
                groups: datedEvent.Gruppen.Select(g => ToReference(g)).ToArray());
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
