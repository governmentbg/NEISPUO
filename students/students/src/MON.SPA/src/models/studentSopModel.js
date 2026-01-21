import { DocumentModel } from '@/models/documentModel.js';

// sopDetails и documents трябва да инициализирани заради реактивитито на vuejs
export class StudentSopModel {
  constructor(obj = {}) {
    this.id = obj.id;
    this.personId = obj.personId;
    this.schoolYear = obj.schoolYear || null;
    this.sopDetails = obj.sopDetails || [{ specialNeedsTypeId: null, specialNeedsSubTypeId: null }];
    this.documents = obj.documents ? obj.documents.map(el => new DocumentModel(el)) : [];
  }
}