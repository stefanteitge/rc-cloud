import { Component, OnInit } from '@angular/core';
import { RaceMeetingListComponent } from '../race-meeting-list/race-meeting-list.component';
import { BeneluxRaceMeetingRepository } from '../../../../shared/race-meeting/repositories/benelux-race-meeting.repository';

@Component({
  selector: 'app-benelux-race-list',
  imports: [RaceMeetingListComponent],
  providers: [BeneluxRaceMeetingRepository],
  templateUrl: './benelux-race-list.component.html',
  styleUrl: './benelux-race-list.component.scss',
})
export class BeneluxRaceListComponent implements OnInit {
  constructor(protected repo: BeneluxRaceMeetingRepository) {}

  ngOnInit() {
    this.repo.fetchAll();
  }
}
