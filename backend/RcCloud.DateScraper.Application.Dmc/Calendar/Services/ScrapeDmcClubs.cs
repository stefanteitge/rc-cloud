﻿using RcCloud.DateScraper.Domain.Clubs;

namespace RcCloud.DateScraper.Application.Dmc.Calendar.Services;

public class ScrapeDmcClubs(ScrapeDmcRaces scrapeDmcRaces)
{
    public async Task<List<Club>> Scrape(int year)
    {
        var racesResult = await scrapeDmcRaces.Scrape(year);

        // TODO: use result here
        if (racesResult.IsFailed)
        {
            return [];
        }

        return racesResult.Value.Select(r => r.Club).OfType<Club>().ToList();
    }
}
