import { Temporal } from 'temporal-polyfill';
import { RaceMeeting } from './types';

export function getRacesInLobith(): RaceMeeting[] {
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
  ];
}
