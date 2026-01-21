import { StudentClassBaseModel } from './studentClassBaseModel';

export class StudentAdditionalClassChangeModel extends StudentClassBaseModel {
  constructor(obj = {}, moment = {}) {
    super(obj, moment);
    this.currentStudentClassId = obj.currentStudentClassId;
  }
}
