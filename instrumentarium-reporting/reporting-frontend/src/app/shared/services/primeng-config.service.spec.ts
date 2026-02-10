import { TestBed } from '@angular/core/testing';

import { PrimengConfigService } from './primeng-config.service';

describe('PrimengConfigService', () => {
  let service: PrimengConfigService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PrimengConfigService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
