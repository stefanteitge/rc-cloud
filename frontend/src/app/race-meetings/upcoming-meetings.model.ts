import {RaceMeetingDto, ReferenceDto} from './race-meeting.repository';

export interface UpcomingRaceMeeting {
  title: string,
  location: string;
  series: string[];
  groups: ReferenceDto[]; // TODO
}

export interface UpcomingDate
{
  date: string;
  raceMeetings: UpcomingRaceMeeting[];
  east: UpcomingRaceMeeting[];
  west: UpcomingRaceMeeting[];
  north: UpcomingRaceMeeting[];
  south: UpcomingRaceMeeting[];
  central: UpcomingRaceMeeting[];
}

export function compileUpcomingMeetings(meetings: RaceMeetingDto[]): UpcomingDate[] {
  const compiled = [] as UpcomingDate[];

  meetings.forEach((meeting) => {
    let existingDate = compiled.find(c => c.date == meeting.date);
    if (existingDate === undefined) {
      existingDate = {
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

    const newMeeting = {
      location: meeting.location,
      series: meeting.series.map(s => s.id),
      groups: meeting.groups,
      title: meeting.title,
    };

    if (newMeeting.groups.length == 0) {
      existingDate.raceMeetings.push(newMeeting);
    }

    if (newMeeting.groups.find(g => g.id == 'east')) {
      existingDate.east.push(newMeeting);
    }

    if (newMeeting.groups.find(g => g.id == 'west')) {
      existingDate.west.push(newMeeting);
    }

    if (newMeeting.groups.find(g => g.id == 'north')) {
      existingDate.north.push(newMeeting);
    }

    if (newMeeting.groups.find(g => g.id == 'south')) {
      existingDate.south.push(newMeeting);
    }

    if (newMeeting.groups.find(g => g.id == 'central')) {
      existingDate.central.push(newMeeting);
    }
  })

  return compiled;
}
