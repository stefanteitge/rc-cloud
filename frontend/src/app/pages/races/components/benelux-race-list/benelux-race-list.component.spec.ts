import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BeneluxRaceListComponent } from './benelux-race-list.component';

describe('BeneluxRaceListComponent', () => {
  let component: BeneluxRaceListComponent;
  let fixture: ComponentFixture<BeneluxRaceListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BeneluxRaceListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BeneluxRaceListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
