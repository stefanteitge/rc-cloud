import { Temporal } from 'temporal-polyfill';
import { RaceMeeting } from './types';
import { getRacesInRonse } from './races-ronse';
import { getRacesInMarrum } from './races-marrum';
import { getRacesInLobith } from './races-lobith';

export function getRaces(): RaceMeeting[] {
  return [...getOtherRaces(), ...getRacesInRonse(), ...getRacesInMarrum(), ...getRacesInLobith()];
}

function getOtherRaces(): RaceMeeting[] {
  return [
    {
      date: new Temporal.PlainDate(2024, 10, 20),
      location: 'Venray',
      classes: ['f1'],
    },
    {
      date: new Temporal.PlainDate(2024, 11, 3),
      location: 'Venray',
      classes: ['f1'],
    },
    {
      date: new Temporal.PlainDate(2024, 11, 3),
      location: 'Branst',
      classes: ['banger'],
    },
    {
      date: new Temporal.PlainDate(2024, 12, 1),
      location: 'Branst',
      classes: ['banger', 'alloy'],
    },
    {
      date: new Temporal.PlainDate(2024, 12, 28),
      location: 'Venray',
      classes: ['banger', 'micra'],
    },
    {
      date: new Temporal.PlainDate(2025, 1, 5),
      location: 'Venray',
      classes: ['f1'],
    },
    {
      date: new Temporal.PlainDate(2025, 1, 5),
      location: 'Branst',
      classes: ['banger', 'caravan'],
    },
    {
      date: new Temporal.PlainDate(2025, 1, 25),
      location: 'Venray',
      classes: ['banger', 'micra'],
    },
    {
      date: new Temporal.PlainDate(2025, 2, 2),
      location: 'Branst',
      classes: ['banger'],
    },
    {
      date: new Temporal.PlainDate(2025, 2, 16),
      location: 'Venray',
      classes: ['f1'],
    },
    {
      date: new Temporal.PlainDate(2025, 3, 2),
      location: 'Branst',
      classes: ['banger'],
    },
    {
      date: new Temporal.PlainDate(2025, 3, 16),
      location: 'Venray',
      classes: ['f1'],
    },
    {
      date: new Temporal.PlainDate(2025, 3, 30),
      location: 'Hengelo',
      classes: ['banger', 'alloy'],
    },
    {
      date: new Temporal.PlainDate(2025, 4, 6),
      location: 'Branst',
      classes: ['banger'],
    },
    {
      date: new Temporal.PlainDate(2025, 9, 14),
      location: 'Branst',
      classes: ['banger'],
    },
    {
      date: new Temporal.PlainDate(2025, 10, 12),
      location: 'Branst',
      classes: ['banger'],
    },
    {
      date: new Temporal.PlainDate(2025, 11, 16),
      location: 'Branst',
      classes: ['banger'],
    },
    {
      date: new Temporal.PlainDate(2025, 12, 14),
      location: 'Branst',
      classes: ['banger'],
    },
  ];
}
