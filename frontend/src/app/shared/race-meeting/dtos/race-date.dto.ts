import { ReferenceDto } from './race-meeting-envelope.dto';

export interface RacePageDto {
  lastUpdate: string;
  dates: RaceDateDto[];
}

export interface UpcomingRace {
  title: string;
  location: string;
  series: ReferenceDto[];
  groups: ReferenceDto[];
  source?: string;
  countryCode?: string;
}

export interface RaceCategoryDto {
  key: string;
  races: UpcomingRace[];
}

export interface RaceDateDto {
  dateEnd: string;
  categories: RaceCategoryDto[];
}
