import Constants from '@/common/constants.js';
import { DocumentModel } from '../documentModel.js';
import DocumentStatuses from '@/common/documentStatuses.js';
import { DropDownModel } from '../dropdownModel.js';
import { StudentEvaluation } from '../studentEvaluation.js';

export class StudentDischargeDocumentModel {
  constructor(obj = {}, moment = {}) {
    this.id = obj.id || null;
    this.noteNumber = obj.noteNumber || null;
    this.noteDate = this.noteDate = (obj.noteDate && moment) ? moment(obj.noteDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.dischargeDate = (obj.dischargeDate && moment) ? moment(obj.dischargeDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.files = obj.files || null;
    this.previousFiles = obj.previousFiles || [];
    this.documents = obj.documents ? obj.documents.map(el => new DocumentModel(el)) : [];
    this.institution = new DropDownModel(obj.institutionDropdownModel) || new DropDownModel();
    this.dischargeReasonType = obj.dischargeReasonTypeDropdownModel || new DropDownModel();
    this.studentEvaluations = obj.evaluations ? obj.evaluations.map(el => new StudentEvaluation(el)) : [];
    this.currentStudentClass = obj.currentStudentClass || null;
    this.currentStudentClassName = obj.currentStudentClassName || '';
    this.status = obj.status ? DocumentStatuses.filter(el => el.value === obj.status)[0] : DocumentStatuses[0];
    this.institutionId = obj.institutionId;
    this.institutionName = obj.institutionName;
    this.schoolYearName = obj.schoolYearName;
  }
}
