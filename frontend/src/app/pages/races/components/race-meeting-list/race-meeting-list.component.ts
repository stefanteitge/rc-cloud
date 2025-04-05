import {Component, OnInit, signal} from '@angular/core';
import { RaceMeetingRepository} from '../../../../shared/race-meeting/repositories/race-meeting.repository';
import {UpcomingDate} from '../../../../shared/race-meeting/domain/upcoming-meetings';
import {NgbAlert} from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-race-meeting-list',
  imports: [NgbAlert],
  templateUrl: './race-meeting-list.component.html',
  styleUrl: './race-meeting-list.component.scss',
  providers: [RaceMeetingRepository]
})
export class RaceMeetingListComponent implements OnInit {
  meetings = signal([] as UpcomingDate[])

  constructor(private repo: RaceMeetingRepository) {
  }

  ngOnInit() {
    this.repo.fetchAll();
  }

  getRetrieved() : string {
    return this.repo.getRetrievedDate();
  }

  getAllDates(): UpcomingDate[] {
    return this.repo.getAll();
  }
}
