import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PieChartTypeReportSettingsComponent } from './pie-chart-type-report-settings.component';

describe('PieChartTypeReportSettingsComponent', () => {
  let component: PieChartTypeReportSettingsComponent;
  let fixture: ComponentFixture<PieChartTypeReportSettingsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PieChartTypeReportSettingsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PieChartTypeReportSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
