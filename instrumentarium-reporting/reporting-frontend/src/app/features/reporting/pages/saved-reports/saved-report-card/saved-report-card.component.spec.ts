import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SavedReportCardComponent } from './saved-report-card.component';

describe('SavedReportCardComponent', () => {
  let component: SavedReportCardComponent;
  let fixture: ComponentFixture<SavedReportCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SavedReportCardComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SavedReportCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
