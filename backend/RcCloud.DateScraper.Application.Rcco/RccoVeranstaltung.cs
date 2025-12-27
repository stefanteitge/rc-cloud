using RcCloud.DateScraper.Domain.Clubs;
using RcCloud.DateScraper.Domain.Races;
using RcCloud.DateScraper.Domain.Series;

namespace RcCloud.DateScraper.Application.Rcco;

public class RccoVeranstaltung(DateOnly datumEnde, string verein, string laufname, string strecke)
{
    public DateOnly DatumEnde { get; } = datumEnde;

    public string Verein { get; } = verein;
    
    public string Laufname { get; } = laufname;

    public string Strecke { get; } = strecke;

    public RaceMeeting ToRaceMeeting(Club club)
    {
        return new RaceMeeting(
            [],
            SeasonReference.Current,
            DatumEnde,
            Verein,
            "de",
            Laufname,
            club.Region is null ? [] : [club.Region],
            club,
            "rccar-online.de");
    }
}
