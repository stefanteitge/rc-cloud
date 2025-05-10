import { Routes } from '@angular/router';
import { ClubListComponent } from './pages/clubs/components/club-list/club-list.component';
import { GermanyRaceListComponent } from './pages/races/components/germany-race-list/germany-race-list.component';
import { BeneluxRaceListComponent } from './pages/races/components/benelux-race-list/benelux-race-list.component';

export const routes: Routes = [
  { path: '', redirectTo: 'germany', pathMatch: 'full' },
  { path: 'germany', component: GermanyRaceListComponent },
  { path: 'benelux', component: BeneluxRaceListComponent },
  { path: 'clubs', component: ClubListComponent },
];
