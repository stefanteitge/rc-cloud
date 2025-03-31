import { Routes } from '@angular/router';
import {RaceMeetingListComponent} from './race-meeting-list/race-meeting-list.component';

export const routes: Routes = [
  { path: '**', component: RaceMeetingListComponent },
];
