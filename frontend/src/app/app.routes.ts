import { Routes } from '@angular/router';
import {RaceMeetingListComponent} from './pages/races/components/race-meeting-list/race-meeting-list.component';
import {ClubListComponent} from './pages/clubs/components/club-list/club-list.component';

export const routes: Routes = [
  { path: '', redirectTo: '/races', pathMatch: 'full' },
  { path: 'races', component: RaceMeetingListComponent },
  { path: 'clubs', component: ClubListComponent },
];
