import { Component } from '@angular/core';
import {ClubListComponent} from '../club-list/club-list.component';

@Component({
  selector: 'app-club-list-page',
  imports: [
    ClubListComponent
  ],
  templateUrl: './club-list-page.component.html',
  styleUrl: './club-list-page.component.scss'
})
export class ClubListPageComponent {

}
