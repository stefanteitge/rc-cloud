import { Component, OnInit } from '@angular/core';
import { ClubRepository } from '../../../../shared/clubs/repositories/club.repository';
import { MatTableModule } from '@angular/material/table';

@Component({
  selector: 'app-club-list',
  imports: [MatTableModule],
  templateUrl: './club-list.component.html',
  styleUrl: './club-list.component.scss',
})
export class ClubListComponent implements OnInit {
  constructor(public repo: ClubRepository) {}

  ngOnInit() {
    this.repo.fetchAll();
  }

  readonly displayedColumns = ['name', 'dmcClubNumber'];
}
