import Constants from '@/common/constants.js';
import { DocumentModel } from '@/models/documentModel.js';

export class StudentAwardModel {
  constructor(obj = {}, moment = {}) {
    this.id = obj.id || null;
    this.personId = obj.personId || null;
    this.date = (obj.date && moment) ? moment(obj.date).format(Constants.DATEPICKER_FORMAT) : '';
    this.orderNumber = obj.orderNumber || null;
    this.description = obj.description || null;
    this.awardTypeId = obj.awardTypeId || null;
    this.awardTypeName = obj.awardTypeName;
    this.institutionId = obj.institutionId || null;
    this.institutionName = obj.institutionName;
    this.awardCategoryId = obj.awardCategoryId || null;
    this.awardCategoryName = obj.awardCategoryName;
    this.founderId = obj.founderId || null;
    this.founderName = obj.founderName;
    this.awardReasonId = obj.awardReasonId || null;
    this.awardReasonName = obj.awardReasonName;
    this.additionalInformation = obj.additionalInformation || null;
    this.documents = obj.documents ? obj.documents.map(el => new DocumentModel(el)) : [];
    this.schoolYear = obj.schoolYear || null;
    this.schoolYearName = obj.schoolYearName;
  }
}
