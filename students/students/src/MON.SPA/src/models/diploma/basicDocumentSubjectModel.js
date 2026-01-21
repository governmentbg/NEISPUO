export class BasicDocumentSubjectModel {
  constructor(obj = {}) {
    this.uid = obj.uid || Math.floor((Math.random()) * 0x10000).toString(16);
    this.id = obj.id;
    this.subjectCanChange = obj.subjectCanChange;
    this.position = obj.position || 1;
    this.subjectId = obj.subjectId;
    this.subjectTypeId = obj.subjectTypeId;
    this.subjectTypeDropdown = obj.subjectTypeDropdown || null;
    this.subjectDropdown = obj.subjectDropdown || null;
  }
}
