import { DocumentModel } from '../documentModel.js';
import Constants from '@/common/constants.js';

export class NewStudentDischargeDocumentModel {
  constructor(obj = {}, moment = {}) {
    this.id = obj.id || null;
    this.personId = obj.personId || null,
    this.noteNumber = obj.noteNumber || null;
    this.noteDate = (obj.noteDate && moment) ? moment(obj.noteDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.dischargeDate = (obj.dischargeDate && moment) ? moment(obj.dischargeDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.dischargeReasonTypeId = obj.dischargeReasonTypeId || null,
    this.studentClassId = obj.studentClassId || null;
    this.studentClassDetails = obj.studentClassDetails || null;
    this.institutionId = obj.institutionId || null;
    this.institutionDetails = obj.institutionDetails || null;
    this.status = obj.status;
    this.documents = obj.documents ? obj.documents.map(el => new DocumentModel(el)) : [];
  }
}
