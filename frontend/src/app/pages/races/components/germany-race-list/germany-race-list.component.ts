import { Component, OnInit } from '@angular/core';
import { RaceMeetingListComponent } from '../race-meeting-list/race-meeting-list.component';
import { RaceMeetingRepository } from '../../../../shared/race-meeting/repositories/race-meeting.repository';

@Component({
  selector: 'app-germany-race-list',
  imports: [RaceMeetingListComponent],
  providers: [RaceMeetingRepository],
  templateUrl: './germany-race-list.component.html',
  styleUrl: './germany-race-list.component.scss',
})
export class GermanyRaceListComponent implements OnInit {
  constructor(protected repo: RaceMeetingRepository) {}

  ngOnInit() {
    this.repo.fetchAll();
  }
}
