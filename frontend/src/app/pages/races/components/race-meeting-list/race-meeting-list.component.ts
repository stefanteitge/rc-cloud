import {Component, OnInit} from '@angular/core';
import {RaceMeetingRepository} from '../../../../shared/race-meeting/repositories/race-meeting.repository';
import {UpcomingDate, UpcomingRace} from '../../../../shared/race-meeting/domain/upcoming-date';
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
  constructor(private repo: RaceMeetingRepository) {
  }

  ngOnInit() {
    this.repo.fetchAll();
  }

  getRetrieved() : string {
    return this.repo.getRetrievedDate();
  }

  getAllDates(): UpcomingDate[] {
    return this.repo.getGermany();
  }

  getRaces(upcomingDate: UpcomingDate, regionId: string): UpcomingRace[] {
    return upcomingDate.columns.find(r => r.key === regionId)?.races.sort((a, b) => a.location.localeCompare(b.location)) ?? [];
  }
}
