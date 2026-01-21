import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SaveReportFormComponent } from './save-report-form.component';

describe('SaveReportFormComponent', () => {
  let component: SaveReportFormComponent;
  let fixture: ComponentFixture<SaveReportFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SaveReportFormComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SaveReportFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
