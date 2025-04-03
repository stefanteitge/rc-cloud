import {RaceMeetingDto} from '../dtos/race-meeting-envelope.dto';
import {UpcomingDate} from '../domain/upcoming-meetings';

export default function compileUpcomingMeetings(meetings: RaceMeetingDto[]): UpcomingDate[] {
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
