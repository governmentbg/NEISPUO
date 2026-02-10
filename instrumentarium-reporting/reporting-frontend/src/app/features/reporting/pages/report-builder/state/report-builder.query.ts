import { Injectable } from '@angular/core';
import { Query } from '@datorama/akita';
import { map } from 'rxjs';
import { ReportBuilderStore, ReportBuilderState } from './report-builder.store';

@Injectable()
export class ReportBuilderQuery extends Query<ReportBuilderState> {
  public report$ = this.select('report');
  public availableDimensions$ = this.select('report').pipe(map((r) => r.dimensions));
  public availableMeasures$ = this.select('report').pipe(map((r) => r.measures));

  constructor(protected override store: ReportBuilderStore) {
    super(store);
  }
}
