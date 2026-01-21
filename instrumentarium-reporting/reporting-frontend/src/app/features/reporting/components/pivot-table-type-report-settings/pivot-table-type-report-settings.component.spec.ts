import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PivotTableTypeReportSettingsComponent } from './pivot-table-type-report-settings.component';

describe('PivotTableTypeReportSettingsComponent', () => {
  let component: PivotTableTypeReportSettingsComponent;
  let fixture: ComponentFixture<PivotTableTypeReportSettingsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PivotTableTypeReportSettingsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PivotTableTypeReportSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
