import { RaceClass } from './types';

export function getOvalClassName(id: string) {
  const clazz = getClasses().find(c => c.id === id);

  if (clazz) {
    return clazz.name;
  }

  return id;
}

export function getClasses(): RaceClass[] {
  return [
    {
      id: 'banger',
      name: 'Banger',
    },
    {
      id: 'alloy',
      name: 'Alloys',
    },
    {
      id: 'f1',
      name: '1:12 F1 Stock Cars',
    },
    {
      id: 'f2',
      name: '1:12 F1 Stock Cars',
    },
    {
      id: 'micra',
      name: 'Micra Stocks',
    },
    {
      id: 'caravan',
      name: 'Caravan',
    },
    {
      id: 'swb',
      name: 'SWB Stocks',
    },
  ];
}
