export class BasicDocumentTemplateSubjectModel {
  constructor(obj = {}) {
    this.uid = obj.uid || Math.floor((Math.random()) * 0x10000).toString(16);
    this.id = obj.id;
    this.subjectCanChange = obj.subjectCanChange;
    this.position = obj.position || 0;
    this.subjectId = obj.subjectId;
    this.subjectTypeId = obj.subjectTypeId;
    this.horarium = obj.horarium;
    this.basicDocumentSubjectId = obj.basicDocumentSubjectId;
    this.basicDocumentPartId = obj.basicDocumentPartId;
    this.templateId = obj.templateId;
    this.isHorariumHidden = obj.isHorariumHidden;
    this.modules = obj.modules || [];

    this.gradeCategory = obj.gradeCategory;
    this.subjectName = obj.subjectName;
    this.showSubjectNamePreview = obj.showSubjectNamePreview;
    this.isProfSubjectHeader = obj.isProfSubjectHeader;

  }
}
