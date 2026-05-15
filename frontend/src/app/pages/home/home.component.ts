import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatRippleModule } from '@angular/material/core';

interface NavCard {
  title: string;
  description: string;
  icon: string;
  flags?: string[];
  route?: string;
  href?: string;
}

interface Section {
  title: string;
  cards: NavCard[];
}

@Component({
  selector: 'app-home',
  imports: [RouterLink, MatCardModule, MatIconModule, MatRippleModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class HomeComponent {
  readonly sections: Section[] = [
    {
      title: 'Races',
      cards: [
        {
          title: 'Race Dates Germany',
          description: 'Upcoming RC racing dates across Germany, grouped by region.',
          icon: 'flag',
          flags: ['de'],
          route: '/germany',
        },
        {
          title: 'Race Dates BeNeLux',
          description: 'Racing events in Belgium, the Netherlands and Luxembourg.',
          icon: 'public',
          flags: ['be', 'nl', 'lu'],
          route: '/benelux',
        },
        {
          title: 'Oval Race Registration',
          description: 'Register for oval / stock car RC racing events.',
          icon: 'how_to_reg',
          flags: ['nl', 'be'],
          href: 'https://stockcar.rc-cloud.de/',
        },
      ],
    },
    {
      title: 'Tools',
      cards: [
        {
          title: 'German RC Clubs',
          description: 'Browse all registered RC clubs and their locations.',
          icon: 'groups',
          route: '/clubs',
        },
        {
          title: 'Gearing Calculator',
          description: 'Calculate final drive ratios for all spur / pinion combinations.',
          icon: 'settings',
          route: '/tools/gearing-calculator',
        },
      ],
    },
  ];
}
