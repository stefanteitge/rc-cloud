namespace RcCloud.Provider.Dmc.Calendar.Domain;

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

    public Praedikat Praedikat { get; } = new(praedikat);

    public string[] Klassen { get; } = klassen;

    public int? OrtsvereinNummer { get; } = ortsvereinNummer;

    public string Verein { get; } = verein;

    public string BemerkungOrt { get; } = bemerkungOrt;

    public string BemerkungLauf { get; } = bemerkungLauf;


    public string[] Ausschreibung { get; } = ausschreibung;


    public string[] Nennung { get; } = nennung;

    public string[] Ergebnis { get; } = ergebnis;

    public string[] Zusatzinfos { get; } = zusatzinfos;

    public bool IsMeeting() => !_nonEventTypes.Contains(Praedikat.Value);
}
