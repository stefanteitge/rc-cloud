import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GermanyRaceListComponent } from './germany-race-list.component';

describe('GermanyRaceListComponent', () => {
  let component: GermanyRaceListComponent;
  let fixture: ComponentFixture<GermanyRaceListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GermanyRaceListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GermanyRaceListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
