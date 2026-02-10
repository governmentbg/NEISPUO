import { Injectable } from '@angular/core';
import { Query } from '@datorama/akita';
import { AvailableReportsStore, AvailableReportsState } from './available-report.store';

@Injectable()
export class AvailableReportsQuery extends Query<AvailableReportsState> {
  availableReports$ = this.select('availableReports');

  constructor(protected override store: AvailableReportsStore) {
    super(store);
  }
}
