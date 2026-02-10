export class LeadTeacherModel {
  constructor(obj = {}) {
    this.id = obj.id;
    this.staffPositionId = obj.staffPositionId;
    this.classId = obj.classId;
    this.classGroupName = obj.classGroupName;
    this.leadTeacherName = obj.leadTeacherName;
  }
}
