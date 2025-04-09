using RcCloud.DateScraper.Application.Dmc.Common.Domain;

namespace RcCloud.DateScraper.Application.Dmc.Calendar.Domain;

public class DmcCalendarEntry(
    DateOnly beginn,
    DateOnly ende,
    string praedikat,
    string[] klassen,
    int? ortsvereinNummer,
    string verein,
    string bemerkungOrt,
    string bemerkungLauf,
    string[] ausschreibung,
    string[] nennung,
    string[] ergebnis,
    string[] zusatzinfos)
{
    private readonly string[] _nonEventTypes = ["SRLDMC", "SKTDMC", "PRAESDMC"];

    public DateOnly Beginn { get; } = beginn;

    public DateOnly Ende { get; } = ende;

    public string Praedikat { get; } = praedikat;

    public string[] Klassen { get; } = klassen;

    public int? OrtsvereinNummer { get; } = ortsvereinNummer;

    public string Verein { get; } = verein;

    public string BemerkungOrt { get; } = bemerkungOrt;

    public string BemerkungLauf { get; } = bemerkungLauf;


    public string[] Ausschreibung { get; } = ausschreibung;


    public string[] Nennung { get; } = nennung;

    public string[] Ergebnis { get; } = ergebnis;

    public string[] Zusatzinfos { get; } = zusatzinfos;

    public bool IsRegionMeeting(DmcRegion regionNumber) => 
        Praedikat == $"FR{(int)regionNumber}DMC" 
        || Praedikat == $"SM{(int)regionNumber}DMC";

    public bool IsMeeting() => !_nonEventTypes.Contains(Praedikat);

    public bool IsSportkreismeisterschaft()
    {
        return Praedikat.StartsWith("SM") && Praedikat.EndsWith("DMC");
    }

    public bool IsFreundschaftsrennen()
    {
        return Praedikat.StartsWith("FR") && Praedikat.EndsWith("DMC");
    }
    
    public bool IsDeutscheMeisterschaft()
    {
        return (Praedikat.StartsWith("DM") || Praedikat.StartsWith("ODM")) && Praedikat.EndsWith("DMC");
    }

    public bool IsShCup()
    {
        return Praedikat.StartsWith("CUPSH") && Praedikat.EndsWith("DMC");
    }
    
    public bool IsTamiyaEurocup()
    {
        return Praedikat.StartsWith("MP-TA") && Praedikat.EndsWith("DMC");
    }
}
