import { Component, input } from '@angular/core';
import { RaceMeetingRepository } from '../../../../shared/race-meeting/repositories/race-meeting.repository';
import { RaceDateDto, UpcomingRace } from '../../../../shared/race-meeting/dtos/race-date.dto';
import { NgbAlert } from '@ng-bootstrap/ng-bootstrap';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-race-meeting-list',
  imports: [NgbAlert, NgIf],
  templateUrl: './race-meeting-list.component.html',
  styleUrl: './race-meeting-list.component.scss',
  providers: [RaceMeetingRepository],
})
export class RaceMeetingListComponent {
  displayColumns = input([] as string[]);
  races = input([] as RaceDateDto[]);
  lastUpdate = input('');

  getLastUpdateDate(): string {
    return this.lastUpdate();
  }

  getAllDates(): RaceDateDto[] {
    return this.races();
  }

  getRacesFromDate(upcomingDate: RaceDateDto, regionId: string): UpcomingRace[] {
    return (
      upcomingDate.categories
        .find(r => r.key === regionId)
        ?.races.sort((a, b) => a.location.localeCompare(b.location)) ?? []
    );
  }
}
