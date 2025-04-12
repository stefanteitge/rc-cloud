import {RaceMeetingDto} from '../dtos/race-meeting-envelope.dto';
import {RaceDate, RaceCategory, UpcomingRace} from '../domain/race-date';

export default function compileUpcomingDates(races: RaceMeetingDto[], displayColumns: string[], byCountry: boolean = false): RaceDate[] {
  const compiled = [] as RaceDate[];

  races.forEach((race) => {
    let existingDate = compiled.find(c => c.dateEnd == race.date);
    if (existingDate === undefined) {
      const newDate = {
        dateEnd: race.date,
        categories: [] as RaceCategory[],
      } as RaceDate;

      displayColumns.forEach((displayColumn) => {
        const r : RaceCategory = { key: displayColumn, races: []};
        newDate.categories.push(r);
      });

      const r : RaceCategory = { key: 'global', races: []};
      newDate.categories.push(r);

      compiled.push(newDate);
      existingDate = newDate;
    }

    const newUpcomingRace = {
      location: race.location,
      series: race.series,
      groups: race.regions,
      title: race.title,
      source: race.source,
    } as UpcomingRace;

    if (byCountry)
    {
      if (newUpcomingRace.countryCode === null) {
        existingDate.categories.find(r => r.key == 'global')?.races.push(newUpcomingRace);
      }

      for (const displayRegion of displayColumns) {
        if (race.countryCode == displayRegion) {
          existingDate.categories.find(r => r.key == displayRegion)?.races.push(newUpcomingRace);
        }
      }
    }
    else
    {
      if (newUpcomingRace.groups.length == 0) {
        existingDate.categories.find(r => r.key == 'global')?.races.push(newUpcomingRace);
      }

      for (const displayRegion of displayColumns) {
        if (race.regions.find(g => g.id == displayRegion)) {
          existingDate.categories.find(r => r.key == displayRegion)?.races.push(newUpcomingRace);
        }
      }
    }

  })

  return compiled;
}
