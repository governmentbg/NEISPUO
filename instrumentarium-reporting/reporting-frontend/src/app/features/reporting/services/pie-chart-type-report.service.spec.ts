import { TestBed } from '@angular/core/testing';

import { PieChartTypeReportService } from './pie-chart-type-report.service';

describe('PieChartTypeReportService', () => {
  let service: PieChartTypeReportService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PieChartTypeReportService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
