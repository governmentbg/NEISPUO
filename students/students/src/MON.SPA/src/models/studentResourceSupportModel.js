import Constants from '@/common/constants.js';
import { DocumentModel } from '@/models/documentModel.js';

export class StudentResourceSupportModel {
  constructor(obj = {}, moment = {}) {
    this.id = obj.id || null;
    this.personId = obj.personId || null,
    this.schoolYear = obj.schoolYear || null,
    this.reportNumber = obj.reportNumber || null,
    this.reportDate = (obj.reportDate && moment) ? moment(obj.reportDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.documents = obj.documents ? obj.documents.map(el => new DocumentModel(el)) : [];
    this.resourceSupports = obj.resourceSupports || [];
  }
}