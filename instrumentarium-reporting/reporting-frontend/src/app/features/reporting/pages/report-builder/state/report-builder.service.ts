import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReportBuilderState, ReportBuilderStore } from './report-builder.store';

@Injectable()
export class ReportBuilderService {
  constructor(private httpClient: HttpClient, private reportBuilderStore: ReportBuilderStore) {}

  updateReport(report: any) {
    this.reportBuilderStore.update((state) => ({
      report: report
    }));
  }
}
