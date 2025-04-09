namespace RcCloud.DateScraper.Application.Rck.SubDomain
{
    public class UndatierterTermin
    {

        public UndatierterTermin(string location, Gruppe gruppe)
        {
            Location = location;
            Gruppen = [gruppe];
        }

        public UndatierterTermin(IGrouping<string, UndatierterTermin> es)
        {
            Location = es.Key;
            Gruppen = es.SelectMany(e => e.Gruppen).ToArray();
        }

        public string Location { get; }
        public Gruppe[] Gruppen { get; }

        public override string ToString()
        {
            return Location + " " + string.Join(", ", Gruppen.Select(g => g.ToString()));
        }
    }
}
