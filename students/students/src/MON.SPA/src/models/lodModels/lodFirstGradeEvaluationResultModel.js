export class LodFirstGradeEvaluationResultModel {
  constructor(obj = {}) {
    this.id = obj.id || null;
    this.personId = obj.personId || null;
    this.schoolYear = obj.schoolYear || null;
    this.studentClassId = obj.studentClassId || null;
    this.gradeResult = obj.gradeResult || null;
  }
}