using RcCloud.DateScraper.Domain;
using RcCloud.DateScraper.Domain.Races;
using RcCloud.DateScraper.Domain.Regions;
using RcCloud.DateScraper.Domain.Series;

namespace RcCloud.DateScraper.Application.Rck.SubDomain
{
    internal static class TerminMapper
    {
        public static RaceMeeting ToDomain(this Renntermin renntermin, SeriesReference series)
        {
            return new RaceMeeting(
                [series],
                SeasonReference.Current,
                renntermin.Date,
                renntermin.Location,
                "de",
                GetTitle(series),
                renntermin.Gruppen.Select(g => ToReference(g)).ToArray(),
                null,
                renntermin.Source);
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

        private static RegionReference ToReference(Gruppe g)
        {
            return g switch
            {
                Gruppe.Mitte => RegionReference.Central,
                Gruppe.Nord => RegionReference.North,
                Gruppe.West => RegionReference.West,
                Gruppe.Sued => RegionReference.South,
                Gruppe.Ost => RegionReference.East,
                _ => throw new ArgumentOutOfRangeException(nameof(g), g, null)
            };
        }
    }
}
