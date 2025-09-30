import { Temporal } from 'temporal-polyfill';
import { OvalRaceMeeting } from './types';

export function getRacesInLobith(): OvalRaceMeeting[] {
  return [
    {
      date: new Temporal.PlainDate(2024, 10, 26),
      location: 'Lobith',
      classes: ['f1'],
    },
    {
      date: new Temporal.PlainDate(2024, 11, 9),
      location: 'Lobith',
      classes: ['f1'],
    },
    {
      date: new Temporal.PlainDate(2024, 12, 8),
      location: 'Lobith',
      classes: ['f1'],
    },
    {
      date: new Temporal.PlainDate(2024, 12, 29),
      location: 'Lobith',
      classes: ['f1'],
    },
    {
      date: new Temporal.PlainDate(2025, 1, 26),
      location: 'Lobith',
      classes: ['f1'],
    },
    {
      date: new Temporal.PlainDate(2025, 2, 22),
      location: 'Lobith',
      classes: ['f1', 'banger'],
    },
    {
      date: new Temporal.PlainDate(2025, 10, 25),
      location: 'Lobith',
      classes: ['f1'],
    },
    {
      date: new Temporal.PlainDate(2025, 11, 16),
      location: 'Lobith',
      classes: ['f1'],
    },
    {
      date: new Temporal.PlainDate(2025, 12, 6),
      location: 'Lobith',
      classes: ['f1'],
    },
    {
      date: new Temporal.PlainDate(2026, 1, 3),
      location: 'Lobith',
      classes: ['f1'],
    },
    {
      date: new Temporal.PlainDate(2026, 2, 1),
      location: 'Lobith',
      classes: ['f1'],
    },
    {
      date: new Temporal.PlainDate(2026, 2, 14),
      location: 'Lobith',
      classes: ['f1'],
    },
    {
      date: new Temporal.PlainDate(2026, 3, 22),
      location: 'Lobith',
      classes: ['f1'],
    },
    {
      date: new Temporal.PlainDate(2026, 3, 29),
      location: 'Lobith',
      classes: ['f1'],
    },
  ];
}
