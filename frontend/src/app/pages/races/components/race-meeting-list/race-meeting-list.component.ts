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
export class RaceMeetingListComponent implements OnInit {
  displayColumns = input([] as string[]);
  dataSet = input('');

  constructor(private repo: RaceMeetingRepository) {
  }

  ngOnInit() {
    this.repo.fetchAll();
  }

  getRetrieved() : string {
    return this.repo.getRetrievedDate(this.dataSet());
  }

  getAllDates(): RaceDate[] {
    return this.repo.getAll(this.dataSet());
  }

  getRaces(upcomingDate: RaceDate, regionId: string): UpcomingRace[] {
    return upcomingDate.categories.find(r => r.key === regionId)?.races.sort((a, b) => a.location.localeCompare(b.location)) ?? [];
  }
}
