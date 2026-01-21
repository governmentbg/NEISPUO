import { TestBed } from '@angular/core/testing';

import { ReportSummaryService } from './report-summary.service';

describe('ReportSummaryService', () => {
  let service: ReportSummaryService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ReportSummaryService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
