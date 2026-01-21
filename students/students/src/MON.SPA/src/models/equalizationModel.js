import { DocumentModel } from './documentModel.js';
export class EqualizationModel {
  constructor(obj = {}) {
    this.id = obj.id,
    this.personId = obj.personId;
    this.reasonId = obj.reasonId;
    this.reasonName = obj.reasonName;
    this.inClass = obj.inClass || "";
    this.basicClassName = obj.basicClassName;
    this.equalizationDetails = obj.equalizationDetails || [];
    this.documents = obj.documents ? obj.documents.map(el => new DocumentModel(el)) : [];
    this.sortOrder = obj.sortOrder;
    this.schoolYear = obj.schoolYear;
    this.schoolYearName = obj.schoolYearName;
    this.horarium = obj.horarium;
    this.sessionId = obj.sessionId || '';
  }
}
