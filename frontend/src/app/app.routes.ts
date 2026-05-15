import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { ClubListComponent } from './pages/clubs/components/club-list/club-list.component';
import { GermanyRaceListComponent } from './pages/races/components/germany-race-list/germany-race-list.component';
import { BeneluxRaceListComponent } from './pages/races/components/benelux-race-list/benelux-race-list.component';
import { GearingCalculatorComponent } from './pages/tools/components/gearing-calculator/gearing-calculator.component';

export const routes: Routes = [
  { path: '', component: HomeComponent, pathMatch: 'full' },
  { path: 'germany', component: GermanyRaceListComponent },
  { path: 'benelux', component: BeneluxRaceListComponent },
  { path: 'clubs', component: ClubListComponent },
  { path: 'tools/gearing-calculator', component: GearingCalculatorComponent },
];
