export class EnrolledStudentsExportModel {
  constructor(obj = {}) {
    this.fileType = obj.fileType;
    this.schoolYear = obj.schoolYear;
    this.month = obj.month;
  }
}