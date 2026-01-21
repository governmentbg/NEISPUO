export class StudentEducation {
    constructor(obj = {}) {
      this.id = obj.id || null;
      this.classId = obj.classId || null;
      this.classNumber = obj.classNumber || '';
      this.educationForm = obj.educationForm || '';
      this.schoolYear = obj.schoolYear || '';
      this.specialty = obj.specialty || '';
      this.grade = obj.grade || '';
      this.group = obj.group || '';
      this.receptionAfter = obj.receptionAfter || '';
      this.recordedInBookSubjectsPage = obj.recordedInBookSubjectsPage || '';
      this.recordedInBookSubjectsNumber = obj.recordedInBookSubjectsNumber || '';
      this.trainingType = obj.trainingType || '';
      this.school = obj.school || '';     
      this.numberInClass = obj.numberInClass || null;     
    }
  }
