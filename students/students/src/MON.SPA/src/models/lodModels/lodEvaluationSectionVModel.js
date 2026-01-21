import { SubjectDetailsDropDownModel } from './subjectDetailsDropdownModel';

export class LodEvaluationSectionVModel {
constructor(obj = {}) {
    this.id = obj.id || '';
    this.orderNum = obj.orderNum || '';
    this.subject = obj.subject || new SubjectDetailsDropDownModel();
    this.sectionVFirstTermGrade = obj.sectionVFirstTermGrade || '';
    this.sectionVSecondTermGrade = obj.sectionVSecondTermGrade || '';
    this.sectionVFinalGrade = obj.sectionVFinalGrade || '';
    this.sectionVSession1Grade = obj.sectionVSession1Grade || '';
    this.sectionVSession2Grade = obj.sectionVSession2Grade || '';
    this.sectionVSession3Grade = obj.sectionVSession3Grade || '';
    this.sectionVHours = obj.sectionVHours || '';
  }
}