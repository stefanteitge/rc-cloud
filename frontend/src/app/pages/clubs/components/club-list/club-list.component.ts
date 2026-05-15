import { Component, computed, OnInit, signal } from '@angular/core';
import { ClubRepository } from '../../../../shared/clubs/repositories/club.repository';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';

const REGION_LABELS: Record<string, string> = {
  north: 'Nord',
  south: 'Süd',
  east: 'Ost',
  west: 'West',
  central: 'Mitte',
};

@Component({
  selector: 'app-club-list',
  imports: [MatTableModule, MatFormFieldModule, MatSelectModule],
  templateUrl: './club-list.component.html',
  styleUrl: './club-list.component.scss',
})
export class ClubListComponent implements OnInit {
  constructor(public repo: ClubRepository) {}

  ngOnInit() {
    this.repo.fetchAll();
  }

  readonly regions = [
    { id: 'north', label: 'Nord' },
    { id: 'south', label: 'Süd' },
    { id: 'east', label: 'Ost' },
    { id: 'west', label: 'West' },
    { id: 'central', label: 'Mitte' },
  ];

  readonly selectedRegion = signal<string | null>(null);

  readonly filteredClubs = computed(() => {
    const region = this.selectedRegion();
    const clubs = this.repo.clubs();
    return region ? clubs.filter(c => c.region === region) : clubs;
  });

  readonly displayedColumns = ['name', 'region', 'dmcClubNumber'];

  regionLabel(id?: string): string {
    return id ? (REGION_LABELS[id] ?? id) : '';
  }
}
