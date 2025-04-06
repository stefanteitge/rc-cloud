import {RaceMeetingDto} from '../dtos/race-meeting-envelope.dto';
import {UpcomingDate, UpcomingDatesColumn, UpcomingRace} from '../domain/upcoming-date';

export default function compileUpcomingDates(races: RaceMeetingDto[], displayColumns: string[], byCountry: boolean = false): UpcomingDate[] {
  const compiled = [] as UpcomingDate[];

  races.forEach((race) => {
    let existingDate = compiled.find(c => c.date == race.date);
    if (existingDate === undefined) {
      const newDate = {
        date: race.date,
        columns: [] as UpcomingDatesColumn[],
      } as UpcomingDate;

      displayColumns.forEach((displayColumn) => {
        const r : UpcomingDatesColumn = { key: displayColumn, races: []};
        newDate.columns.push(r);
      });

      const r : UpcomingDatesColumn = { key: 'global', races: []};
      newDate.columns.push(r);

      compiled.push(newDate);
      existingDate = newDate;
    }

    const newUpcomingRace = {
      location: race.location,
      series: race.series.map(s => s.id),
      groups: race.regions,
      title: race.title,
      source: race.source,
    } as UpcomingRace;

    if (byCountry)
    {
      if (newUpcomingRace.countryCode === null) {
        existingDate.columns.find(r => r.key == 'global')?.races.push(newUpcomingRace);
      }

      for (const displayRegion of displayColumns) {
        if (race.countryCode == displayRegion) {
          existingDate.columns.find(r => r.key == displayRegion)?.races.push(newUpcomingRace);
        }
      }
    }
    else
    {
      if (newUpcomingRace.groups.length == 0) {
        existingDate.columns.find(r => r.key == 'global')?.races.push(newUpcomingRace);
      }

      for (const displayRegion of displayColumns) {
        if (race.regions.find(g => g.id == displayRegion)) {
          existingDate.columns.find(r => r.key == displayRegion)?.races.push(newUpcomingRace);
        }
      }
    }

  })

  return compiled;
}
