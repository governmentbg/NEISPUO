import { TestBed } from '@angular/core/testing';

import { ReportExcelExportService } from './report-excel-export.service';

describe('ReportExcelExportService', () => {
  let service: ReportExcelExportService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ReportExcelExportService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
