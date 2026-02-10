import { TestBed } from '@angular/core/testing';

import { TableTypeReportService } from './table-type-report.service';

describe('TableTypeReportService', () => {
  let service: TableTypeReportService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TableTypeReportService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
