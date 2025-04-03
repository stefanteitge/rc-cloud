import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RaceMeetingListComponent } from './race-meeting-list.component';

describe('RaceMeetingListComponent', () => {
  let component: RaceMeetingListComponent;
  let fixture: ComponentFixture<RaceMeetingListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RaceMeetingListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RaceMeetingListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
