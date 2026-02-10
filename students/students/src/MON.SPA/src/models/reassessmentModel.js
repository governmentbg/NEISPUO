import { DocumentModel } from './documentModel.js';
export class ReassessmentModel {
  constructor(obj = {}) {
    this.id = obj.id,
    this.personId = obj.personId;
    this.reasonId = obj.reasonId;
    this.reason = obj.reason;
    this.reasonName = obj.reasonName;
    this.inClass = obj.inClass || "";
    this.basicClassName = obj.basicClassName;
    this.reassessmentDetails = obj.reassessmentDetails || [];
    this.documents = obj.documents ? obj.documents.map(el => new DocumentModel(el)) : [];
    this.sortOrder = obj.sortOrder;
    this.schoolYear = obj.schoolYear;
    this.schoolYearName = obj.schoolYearName;
  }
}