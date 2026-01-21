import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PieChartTypeReportComponent } from './pie-chart-type-report.component';

describe('PieChartTypeReportComponent', () => {
  let component: PieChartTypeReportComponent;
  let fixture: ComponentFixture<PieChartTypeReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PieChartTypeReportComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PieChartTypeReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
