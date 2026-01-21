import { DocumentModel } from '@/models/documentModel.js';
export class StudentAdmissionDocumentPermissionRequestModel {
  constructor(obj = {}) {
    this.id = obj.id;
    this.personId = obj.personId;
    this.requestingInstitutionId = obj.requestingInstitutionId;
    this.authorizingInstitutionId = obj.authorizingInstitutionId;
    this.note = obj.note;
    this.isPermissionGranted = obj.isPermissionGranted;
    this.documents = obj.documents ? obj.documents.map(el => new DocumentModel(el)) : [];
  }
}
