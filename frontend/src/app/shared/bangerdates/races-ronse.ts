import { Temporal } from 'temporal-polyfill';
import { OvalRaceMeeting } from './types';

export function getRacesInRonse(): OvalRaceMeeting[] {
  return [
    {
      date: new Temporal.PlainDate(2024, 10, 27),
      location: 'Ronse',
      classes: ['banger', 'caravan'],
    },
    {
      date: new Temporal.PlainDate(2024, 11, 17),
      location: 'Ronse',
      classes: ['banger', 'swb'],
    },
    {
      date: new Temporal.PlainDate(2025, 2, 16),
      location: 'Ronse',
      classes: ['banger'],
    },
    {
      date: new Temporal.PlainDate(2025, 3, 16),
      location: 'Ronse',
      classes: ['banger'],
    },
    {
      date: new Temporal.PlainDate(2025, 5, 18),
      location: 'Ronse',
      classes: ['banger'],
    },
    {
      date: new Temporal.PlainDate(2025, 6, 22),
      location: 'Ronse',
      classes: ['banger'],
    },
    {
      date: new Temporal.PlainDate(2025, 9, 21),
      location: 'Ronse',
      classes: ['banger'],
    },
    {
      date: new Temporal.PlainDate(2025, 10, 26),
      location: 'Ronse',
      classes: ['banger'],
    },
    {
      date: new Temporal.PlainDate(2025, 11, 23),
      location: 'Ronse',
      classes: ['banger'],
    },
  ];
}
