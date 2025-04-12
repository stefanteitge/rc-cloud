import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ClubListPageComponent } from './club-list-page.component';

describe('ClubListPageComponent', () => {
  let component: ClubListPageComponent;
  let fixture: ComponentFixture<ClubListPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ClubListPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ClubListPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
