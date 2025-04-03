import { ReferenceDto } from '../dtos/race-meeting-envelope.dto';

export interface UpcomingRaceMeeting {
  title: string,
  location: string;
  series: string[];
  groups: ReferenceDto[];
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

