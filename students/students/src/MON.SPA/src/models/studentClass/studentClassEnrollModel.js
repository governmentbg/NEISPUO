import Constants from '@/common/constants.js';
import { StudentClassEditModel } from "./studentClassEditModel";
import { StudentClassDualFormCompanyModel } from "./studentClassDualFormCompanyModel";

export class StudentClassEnrollModel extends StudentClassEditModel {
    constructor(obj = {}, moment = {}) {
      super(obj);
      this.internationalProtectionStatus = obj.internationalProtectionStatus || '';
      this.isHourlyOrganization = obj.isHourlyOrganization || '';
      this.isFTACOutsourced = obj.IsFTACOutsourced || '';
      this.currentStudentClassId = obj.currentStudentClassId || '';
      this.studentEduFormId = obj.studentEduFormId;
      this.studentSpecialityId = obj.studentSpecialityId;
      this.studentProfessionId = obj.studentProfessionId;
      this.basicClassId = obj.basicClassId;
      this.selectedClassTypeId = obj.selectedClassTypeId;
      this.isNotPresentForm = obj.isNotPresentForm;
      this.enrollmentDate = (obj.enrollmentDate && moment) ? moment(obj.enrollmentDate).format(Constants.DATEPICKER_FORMAT) : '';
      this.entryDate = (obj.entryDate && moment) ? moment(obj.entryDate).format(Constants.DATEPICKER_FORMAT) : '';
      this.dischargeDate = (obj.dischargeDate && moment) ? moment(obj.dischargeDate).format(Constants.DATEPICKER_FORMAT) : '';
      this.dualFormCompanies = obj.dualFormCompanies;

      this.dualFormCompanies = obj.dualFormCompanies ? obj.dualFormCompanies.map(el => new StudentClassDualFormCompanyModel(el, moment)) : null;
    }
}
