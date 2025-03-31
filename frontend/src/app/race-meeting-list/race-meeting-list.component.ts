import {Component, OnInit, signal} from '@angular/core';
import {RaceMeetingDto, RaceMeetingRepository} from '../race-meetings/race-meeting.repository';
import {HttpClient} from '@angular/common/http';
import {MatChip} from '@angular/material/chips';
import {
  MatCell,
  MatCellDef,
  MatColumnDef,
  MatHeaderCell,
  MatHeaderCellDef,
  MatHeaderRow, MatHeaderRowDef, MatRow, MatRowDef,
  MatTable
} from '@angular/material/table';
import {UpcomingDate} from '../race-meetings/upcoming-meetings.model';

@Component({
  selector: 'app-race-meeting-list',
  imports: [MatChip, MatTable, MatCell, MatHeaderCell, MatColumnDef, MatHeaderRow, MatHeaderCellDef, MatCellDef, MatHeaderRowDef, MatRow, MatRowDef],
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

  getAllDates(): UpcomingDate[] {
    return this.repo.getAll();
  }
}
