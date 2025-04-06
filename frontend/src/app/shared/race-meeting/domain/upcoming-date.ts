import { ReferenceDto } from '../dtos/race-meeting-envelope.dto';

export interface UpcomingRace {
  title: string,
  location: string;
  series: string[];
  groups: ReferenceDto[];
  source?: string;
}

export interface UpcomingDatesColumn
{
  key: string,
  races: UpcomingRace[];
}

export interface UpcomingDate
{
  date: string;
  columns: UpcomingDatesColumn[];
}

