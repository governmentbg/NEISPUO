import Constants from '@/common/constants.js';

export class StudentClassBaseModel {
  constructor(obj = {},  moment = {}) {
    this.id = obj.id;
    this.personId = obj.personId;
    this.schoolYear = obj.schoolYear;
    this.classId = obj.classId;
    this.selectedClassTypeId = obj.selectedClassTypeId;
    this.enrollmentDate = (obj.enrollmentDate && moment) ? moment(obj.enrollmentDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.dischargeDate = (obj.dischargeDate && moment) ? moment(obj.dischargeDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.positionId = obj.positionId;
    this.profession = obj.profession;
    this.eduForm = obj.eduForm;
    this.classGroup = obj.classGroup;
  }
}
