import { BasicDocumentPartModel } from '@/models/diploma/basicDocumentPartModel';
import { DropDownModel } from '@/models/dropdownModel';

export class DiplomaTemplateModel {
  constructor(obj = {}) {
    this.templateId = obj.templateId || null;
    this.institution = obj.institution || new DropDownModel();
    this.name = obj.name || '';
    this.basciDocumentId = obj.basciDocumentId;
    this.contents = Object.keys(obj).length > 0 ? JSON.parse(obj.contents) : '';
    this.subjectContents = Object.keys(obj).length > 0 ? JSON.parse(obj.subjectContents) : [];
    this.documentTemplatePartsWithSubjects = obj.documentTemplatePartsWithSubjects ? obj.documentTemplatePartsWithSubjects.map(el => new BasicDocumentPartModel(el)) : [];
  }
}
