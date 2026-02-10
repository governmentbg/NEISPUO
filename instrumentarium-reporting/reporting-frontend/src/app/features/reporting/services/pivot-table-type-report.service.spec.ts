import { TestBed } from '@angular/core/testing';

import { PivotTableTypeReportService } from './pivot-table-type-report.service';

describe('PivotTableTypeReportService', () => {
  let service: PivotTableTypeReportService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PivotTableTypeReportService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
