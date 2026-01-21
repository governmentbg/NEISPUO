import Constants from '@/common/constants.js';
import { DocumentModel } from '../documentModel.js';
import { StudentEvaluation } from '../studentEvaluation.js';

export class StudentRelocationDocumentModel {
  constructor(obj = {}, moment = {}) {
    this.id = obj.id || null;
    this.noteNumber = obj.noteNumber || null;
    this.noteDate = (obj.noteDate && moment) ? moment(obj.noteDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.dischargeDate = (obj.dischargeDate && moment) ? moment(obj.dischargeDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.status = obj.status,
    this.institutionId = obj.institutionId;
    this.currentStudentClassId = obj.currentStudentClassId;
    this.relocationReasonTypeId = obj.relocationReasonTypeId;
    this.documents = obj.documents ? obj.documents.map(el => new DocumentModel(el)) : [];
    this.studentEvaluations = obj.evaluations ? obj.evaluations.map(el => new StudentEvaluation(el)) : [];
    this.admissionDocumentModels = obj.admissionDocumentModels || [];
    this.ruoOrderNumber = obj.ruoOrderNumber || '';
    this.ruoOrderDate = (obj.ruoOrderDate && moment) ? moment(obj.ruoOrderDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.canBeModified = obj.canBeModified;
    this.currentStudentClassName = obj.currentStudentClassName;
    this.institutionName = obj.institutionName,
    this.sendingInstitutionId = obj.sendingInstitutionId;
    this.sendingInstitution = obj.sendingInstitution;
    this.statusName = obj.statusName;
  }
}
