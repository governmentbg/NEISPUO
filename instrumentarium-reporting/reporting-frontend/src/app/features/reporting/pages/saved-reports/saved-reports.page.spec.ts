import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SavedReportsPage } from './saved-reports.page';

describe('SavedReportsPage', () => {
  let component: SavedReportsPage;
  let fixture: ComponentFixture<SavedReportsPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [SavedReportsPage]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SavedReportsPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
