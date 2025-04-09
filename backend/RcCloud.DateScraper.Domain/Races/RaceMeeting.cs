using RcCloud.DateScraper.Domain.Clubs;
using RcCloud.DateScraper.Domain.Regions;
using RcCloud.DateScraper.Domain.Series;

namespace RcCloud.DateScraper.Domain.Races;

public class RaceMeeting(
    SeriesReference[] series,
    SeasonReference season,
    DateOnly date,
    string location,
    string? countryCode,
    string title,
    RegionReference[] regions,
    Club? club,
    string? source)
{
    public SeriesReference[] Series { get; } = series;

    public SeasonReference Season { get; } = season;

    public DateOnly Date { get; } = date;

    public string Location { get; } = location;
    
    public string? CountryCode { get; } = countryCode;

    public string Title { get; } = title;

    public RegionReference[] Regions { get; } = regions;
    
    public Club? Club { get; } = club;
    
    public string? Source { get; } = source;
}
