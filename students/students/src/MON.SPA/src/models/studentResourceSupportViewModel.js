//import { StudentResourceSupportBase } from './studentResourceSupportModel.js';
import { StudentResourceSupportReportModel } from './studentResourceSupportReportModel.js';

import moment from 'moment';

export class StudentResourceSupportViewModel {
  constructor(obj = {}) {

    this.hasResourceSupport = obj.hasResourceSupport || false;
    this.resourceSupportReports = obj.resourceSupportReports ? obj.resourceSupportReports.map(x => new StudentResourceSupportReportModel(x, moment)) : [];
    this.resourceSupportReportsPreviousYears = obj.resourceSupportReportsPreviousYears ? obj.resourceSupportReportsPreviousYears.map(x => new StudentResourceSupportReportModel(x, moment)) : [];
  }
}