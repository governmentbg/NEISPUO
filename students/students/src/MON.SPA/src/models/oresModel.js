import { DocumentModel } from '@/models/documentModel.js';
import Constants from '@/common/constants.js';

export class OresModel {
  constructor(obj = {}, moment = {}) {
    this.id = obj.id;
    this.oresTypeId = obj.oresTypeId;
    this.oresTypeName = obj.oresTypeName;
    this.description = obj.description;
    this.personId = obj.personId;
    this.classId = obj.classId;
    this.institutionId = obj.institutionId;
    this.startDate = (obj.startDate && moment) ? moment(obj.startDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.endDate = (obj.endDate && moment) ? moment(obj.endDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.documents = obj.documents ? obj.documents.map(el => new DocumentModel(el)) : [];
  }
}
