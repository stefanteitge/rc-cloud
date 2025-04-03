export interface RaceMeetingEnvelopeDto
{
  retrievedDate: string;
  raceMeetings: RaceMeetingDto[];
}

export interface ReferenceDto {
  id: string;
}

export interface RaceMeetingDto {
  date: string;
  location: string;
  title: string
  series: ReferenceDto[];
  groups: ReferenceDto[];
}
