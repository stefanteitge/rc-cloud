﻿using System.Linq;

namespace RcCloud.DateScraper.Rck.Application.SubDomain
{
    public class UndatedEvent
    {

        public UndatedEvent(string location, Gruppe gruppe)
        {
            Location = location;
            Gruppen = [gruppe];
        }

        public UndatedEvent(IGrouping<string, UndatedEvent> es)
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
