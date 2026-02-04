namespace RcCloud.DateScraper.Application.Rck.Domain;

public class Renntermin
{
    public Renntermin(DateOnly date, IGrouping<string, UndatierterTermin> es, string source)
    {
        Location = es.Key;
        Gruppen = es.SelectMany(e => e.Gruppen).ToArray();
        Date = date;
        Source = source;
    }

    public string Location { get; }

    public Gruppe[] Gruppen { get; }

    public DateOnly Date { get; }
    
    public string Source { get; }

    public override string ToString()
    {
        return Location + " " + string.Join(", ", Gruppen.Select(g => g.ToString()));
    }
}
