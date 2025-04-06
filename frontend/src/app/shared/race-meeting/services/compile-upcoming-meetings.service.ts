import {RaceMeetingDto} from '../dtos/race-meeting-envelope.dto';
import {UpcomingDate} from '../domain/upcoming-meetings';

export default function compileUpcomingMeetings(meetings: RaceMeetingDto[]): UpcomingDate[] {
  const compiled = [] as UpcomingDate[];

  meetings.forEach((meeting) => {
    let existingDate = compiled.find(c => c.date == meeting.date);
    if (existingDate === undefined) {
      existingDate= {
        date: meeting.date,
        raceMeetings: [],
        east: [],
        west: [],
        north: [],
        south: [],
        central: [],
      };
      compiled.push(existingDate);
    }

    const newRace = {
      location: meeting.location,
      series: meeting.series.map(s => s.id),
      groups: meeting.regions,
      title: meeting.title,
      source: meeting.source,
    };

    if (newRace.groups.length == 0) {
      existingDate.raceMeetings.push(newRace);
    }

    if (newRace.groups.find(g => g.id == 'east')) {
      existingDate.east.push(newRace);
    }

    if (newRace.groups.find(g => g.id == 'west')) {
      existingDate.west.push(newRace);
    }

    if (newRace.groups.find(g => g.id == 'north')) {
      existingDate.north.push(newRace);
    }

    if (newRace.groups.find(g => g.id == 'south')) {
      existingDate.south.push(newRace);
    }

    if (newRace.groups.find(g => g.id == 'central')) {
      existingDate.central.push(newRace);
    }

    existingDate.raceMeetings.sort((a, b) => a.location.localeCompare(b.location));
    existingDate.east.sort((a, b) => a.location.localeCompare(b.location));
    existingDate.west.sort((a, b) => a.location.localeCompare(b.location));
    existingDate.north.sort((a, b) => a.location.localeCompare(b.location));
    existingDate.south.sort((a, b) => a.location.localeCompare(b.location));
    existingDate.central.sort((a, b) => a.location.localeCompare(b.location));
  })

  return compiled;
}
