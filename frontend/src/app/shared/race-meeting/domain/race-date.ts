import { ReferenceDto } from '../dtos/race-meeting-envelope.dto';

export interface GermanyPage {
  lastUpdate: string;
  dates: RaceDate[];
}

export interface UpcomingRace {
  title: string,
  location: string;
  series: ReferenceDto[];
  groups: ReferenceDto[];
  source?: string;
  countryCode?: string;
}

// RaceCategoryDto

export interface RaceCategory
{
  key: string,
  races: UpcomingRace[];
}

// RaceDateDto
export interface RaceDate
{
  dateEnd: string;
  categories: RaceCategory[];
}

