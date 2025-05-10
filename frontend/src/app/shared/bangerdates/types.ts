import { Temporal } from 'temporal-polyfill';

export interface RaceMeeting {
  date: Temporal.PlainDate;
  location: string;
  classes: string[];
}

export interface RaceMeeting2 {
  date: Temporal.PlainDate;
  location: string;
  classes: RaceClass[];
}

export interface RaceDay2 {
  date: Temporal.PlainDate;
  race: RaceMeeting2;
}

export interface RaceClass {
  id: string;
  name: string;
}
