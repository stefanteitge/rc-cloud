import { Temporal } from 'temporal-polyfill';
import { OvalRaceMeeting } from './types';

export function getRacesInMarrum(): OvalRaceMeeting[] {
  return [
    {
      date: new Temporal.PlainDate(2024, 11, 17),
      location: 'Marrum',
      classes: ['banger'],
    },
    {
      date: new Temporal.PlainDate(2024, 12, 15),
      location: 'Marrum',
      classes: ['f1'],
    },
    {
      date: new Temporal.PlainDate(2025, 1, 12),
      location: 'Marrum',
      classes: ['banger'],
    },
    {
      date: new Temporal.PlainDate(2025, 2, 9),
      location: 'Marrum',
      classes: ['f1'],
    },
    {
      date: new Temporal.PlainDate(2025, 3, 8),
      location: 'Marrum',
      classes: ['f1', 'banger'],
    },
    {
      date: new Temporal.PlainDate(2025, 3, 9),
      location: 'Marrum',
      classes: ['banger'],
    },
    {
      date: new Temporal.PlainDate(2025, 4, 6),
      location: 'Marrum',
      classes: ['f1'],
    },
    {
      date: new Temporal.PlainDate(2025, 7, 11),
      location: 'Marrum',
      classes: ['banger'],
    },
  ];
}
