export class StudentEducationModel {
  constructor(obj = {}) {
    this.id = obj.id || null;
    this.schoolYear = obj.schoolYear || '';
    this.educationForm = obj.educationForm || '';
    this.basicClassId = obj.basicClassId || '';
    this.specialty = obj.specialty || '';
  }
}