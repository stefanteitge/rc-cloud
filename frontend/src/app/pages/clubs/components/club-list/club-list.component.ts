import {Component, OnInit} from '@angular/core';
import {ClubRepository} from '../../../../shared/clubs/repositories/club.repository';

@Component({
  selector: 'app-club-list',
  imports: [],
  templateUrl: './club-list.component.html',
  styleUrl: './club-list.component.scss'
})
export class ClubListComponent implements OnInit {
  constructor(public repo: ClubRepository) { }

  ngOnInit() {
    this.repo.fetchAll();
  }
}
