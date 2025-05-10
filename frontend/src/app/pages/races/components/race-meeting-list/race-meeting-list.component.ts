import {Component, input, OnInit} from '@angular/core';
import {RaceMeetingRepository} from '../../../../shared/race-meeting/repositories/race-meeting.repository';
import {RaceDate, UpcomingRace} from '../../../../shared/race-meeting/domain/race-date';
import {NgbAlert} from '@ng-bootstrap/ng-bootstrap';
import {NgIf} from '@angular/common';

@Component({
  selector: 'app-race-meeting-list',
  imports: [NgbAlert, NgIf],
  templateUrl: './race-meeting-list.component.html',
  styleUrl: './race-meeting-list.component.scss',
  providers: [RaceMeetingRepository]
})
export class RaceMeetingListComponent {
  displayColumns = input([] as string[]);
  races = input([] as RaceDate[]);
  lastUpdate = input('');

  constructor() {
  }

  getLastUpdateDate() : string {
    return this.lastUpdate();
  }

  getAllDates(): RaceDate[] {
    return this.races();
  }

  getRacesFromDate(upcomingDate: RaceDate, regionId: string): UpcomingRace[] {
    return upcomingDate.categories.find(r => r.key === regionId)?.races.sort((a, b) => a.location.localeCompare(b.location)) ?? [];
  }
}
