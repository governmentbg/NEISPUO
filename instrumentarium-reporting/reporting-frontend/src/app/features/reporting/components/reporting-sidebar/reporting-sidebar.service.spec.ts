import { TestBed } from '@angular/core/testing';

import { ReportingSidebarService } from './reporting-sidebar.service';

describe('ReportingSidebarService', () => {
  let service: ReportingSidebarService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ReportingSidebarService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
