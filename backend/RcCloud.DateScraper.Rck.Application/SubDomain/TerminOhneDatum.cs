using System.Linq;

namespace RcCloud.DateScraper.Rck.Application.SubDomain
{
    public class DatedEvent
    {
        public DatedEvent(DateOnly date, IGrouping<string, UndatedEvent> es)
        {
            Location = es.Key;
            Gruppen = es.SelectMany(e => e.Gruppen).ToArray();
            Date = date;
        }

        public string Location { get; }

        public Gruppe[] Gruppen { get; }

        public DateOnly Date { get; }

        public override string ToString()
        {
            return Location + " " + string.Join(", ", Gruppen.Select(g => g.ToString()));
        }
    }
}
