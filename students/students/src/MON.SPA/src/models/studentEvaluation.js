import {uuid} from 'vue-uuid';
export class StudentEvaluation {
  constructor(obj = {}) {
    this.id = obj.id || null;
    this.dropdownModel = obj.dropdownModel || null;
    this.twoYearsAgoEvaluation = obj.twoYearsAgoEvaluation || null;
    this.oneYearAgoEvaluation = obj.oneYearAgoEvaluation || null;
    this.firstTermEvaluation = obj.firstTermEvaluation || null;
    this.secondTermEvaluation = obj.secondTermEvaluation || null;
    this.annualEvaluation = obj.annualEvaluation || null;
    this.index = obj.index || null;
    this.curriculumPartId = obj.curriculumPartId || null;
    this.sortOrder = obj.sortOrder;
    this.uid = uuid.v4();
  }
}
