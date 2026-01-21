import { SubjectDetailsDropDownModel } from './subjectDetailsDropdownModel';

export class LodFirstGradeEvaluationModel {
  constructor(obj = {}) {
    this.id = obj.id || null;
    this.orderNum = obj.orderNum || null;
    this.subject = obj.subject || new SubjectDetailsDropDownModel();
    this.firstTermGrade = obj.firstTermGrade || null;
    this.secondTermGrade = obj.secondTermGrade || null;
    this.finalGrade = obj.finalGrade || null;
  }
}