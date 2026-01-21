import { StudentResourceSupportSpecialist } from './studentResourceSupportSpecialist.js';
import { StudentResourceSupportDocumentModel } from  './studentResourceSupportDocumentModel.js';
import Constants from '@/common/constants.js';

export class StudentResourceSupportReportModel{
    constructor(obj = {}, moment = {}) {
        this.id = obj.id || 0;
        this.reportDate = (obj.reportDate && moment) ? moment(obj.reportDate).format(Constants.DATEPICKER_FORMAT) : '';
        this.reportNumber = obj.reportNumber || '';
        this.personId = obj.personId || 0;
        this.year = obj.year || 0;
        this.resourceSupportDetails = obj.resourceSupportDetails ? obj.resourceSupportDetails.map(el => new StudentResourceSupportModel(el)) : [];
        this.documentsToAdd = [];
        this.resourceSupportDocuments = obj.resourceSupportDocuments ? obj.resourceSupportDocuments.map(el => new StudentResourceSupportDocumentModel(el)) : [];
    }
}

export class StudentResourceSupportModel {
  constructor(obj = {}) {
    this.id = obj.id || 0;
    this.resourceSupportSpecialists = obj.resourceSupportSpecialists ? obj.resourceSupportSpecialists.map(el => new StudentResourceSupportSpecialist(el)) : [];
    this.menu = obj.menu || false;
    this.resourceSupportTypeId = obj.resourceSupportTypeId || '';
    this.resourceSupportReportId = obj.resourceSupportReportId || '';
  }
}

