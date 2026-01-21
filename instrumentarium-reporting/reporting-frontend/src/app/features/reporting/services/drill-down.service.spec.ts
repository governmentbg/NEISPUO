import { TestBed } from '@angular/core/testing';

import { DrillDownService } from './drill-down.service';

describe('DrillDownService', () => {
  let service: DrillDownService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DrillDownService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
