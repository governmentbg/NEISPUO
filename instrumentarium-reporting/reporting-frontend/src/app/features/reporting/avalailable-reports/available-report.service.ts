import { Injectable } from '@angular/core';
import { AvailableReportsStore } from './available-report.store';

@Injectable()
export class AvailableReportsService {
  constructor(private availableReportsStore: AvailableReportsStore) {}

  updateAvailableReports(availableReports: any[]) {
    this.availableReportsStore.update((state) => ({
      availableReports: [...availableReports]
    }));
  }
}
