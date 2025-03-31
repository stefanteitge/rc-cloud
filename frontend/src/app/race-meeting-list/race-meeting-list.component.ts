import {Component, OnInit, signal} from '@angular/core';
import {RaceMeeting, RaceMeetingRepository} from '../race-meetings/race-meeting.repository';
import {HttpClient} from '@angular/common/http';

@Component({
  selector: 'app-race-meeting-list',
  imports: [],
  templateUrl: './race-meeting-list.component.html',
  styleUrl: './race-meeting-list.component.scss',
  providers: [RaceMeetingRepository]
})
export class RaceMeetingListComponent implements OnInit {
  meetings = signal([] as RaceMeeting[])

  constructor(private repo: RaceMeetingRepository) {
  }

  ngOnInit() {
    this.repo.fetchAll();
  }

  getAllMeetings() {
    return this.repo.getAll();
  }
}
