export interface RaceMeetingEnvelopeDto
{
  lastUpdate: string;
  races: RaceMeetingDto[];
}

export interface ReferenceDto {
  id: string;
}

export interface RaceMeetingDto {
  countryCode?: string;
  date: string;
  location: string;
  title: string
  series: ReferenceDto[];
  regions: ReferenceDto[];
  source?: string;
}
