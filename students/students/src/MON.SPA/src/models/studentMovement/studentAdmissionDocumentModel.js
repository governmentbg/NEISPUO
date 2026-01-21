import Constants from '@/common/constants.js';
import { DocumentModel } from '../documentModel.js';
import { DropDownModel } from '../dropdownModel.js';

export class StudentAdmissionDocumentModel {
  constructor(obj = {}, moment = {}) {
    this.id = obj.id || '';
    this.admissionDate = (obj.admissionDate && moment) ? moment(obj.admissionDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.noteNumber = obj.noteNumber || '';
    this.noteDate = (obj.noteDate && moment) ? moment(obj.noteDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.dischargeDate = (obj.dischargeDate && moment) ? moment(obj.dischargeDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.status = obj.status,
    this.statusName = obj.statusName,
    this.documents = obj.documents ? obj.documents.map(el => new DocumentModel(el)) : [];
    this.institutionId = obj.institutionId,
    this.institutionName = obj.institutionName,
    this.relocationDocumentId = obj.relocationDocumentId || '';
    this.relocationDocument = obj.relocationDocumentId || new DropDownModel({ value: null, text: 'Без удостоверение за преместване', name: 'Без удостоверение за преместване' });
    this.admissionReasonTypeId = obj.admissionReasonTypeId || null;
    this.admissionReasonTypeName = obj.admissionReasonTypeName,
    this.isReferencedInStudentClass = obj.isReferencedInStudentClass || null;
    this.position = obj.position;
    this.positionName = obj.positionName;
    this.classEnrolledIn = obj.classEnrolledIn || '';
    this.canBeModified = obj.canBeModified;
    this.canBeEnrolled = obj.canBeEnrolled;
    this.createdBySysUserId = obj.createdBySysUserId;
    this.hasHealthStatusDocument =  obj.hasHealthStatusDocument || false;
    this.hasImmunizationStatusDocument =  obj.hasImmunizationStatusDocument || false;
  }
}
