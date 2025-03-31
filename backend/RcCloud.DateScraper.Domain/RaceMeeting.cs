namespace RcCloud.DateScraper.Domain
{
    public class RaceMeeting(
        SeriesReference series,
        SeasonReference season,
        DateOnly date,
        string location,
        GroupReference[] groups)
    {
        public SeriesReference Series { get; } = series;

        public SeasonReference Season { get; } = season;

        public DateOnly Date { get; } = date;

        public string Location { get; } = location;

        public GroupReference[] Groups { get; } = groups;
    }
}
