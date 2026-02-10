import Constants from '@/common/constants.js';
import { DropDownModel } from '@/models/dropdownModel';

export class MonthlyBenefit {
    constructor(obj = {},  moment = {}) {
        this.id = obj.id;
        this.personId = obj.personId;
        this.firstName = obj.firstName;
        this.lastName = obj.lastName;
        this.middleName = obj.middleName;
        this.studentIdentificationType = obj.studentIdentificationType;
        this.identification = obj.identification;
        this.schoolYear = obj.schoolYear;
        this.month = obj.month;
        this.reason = obj.reason;
        this.absenceCount  = obj.absenceCount;
        this.aspStatus = obj.aspStatus;
        this.aspStatusName = obj.aspStatusName;
        this.daysCount = obj.daysCount;
        this.institutionId = obj.institutionId;
        this.institutionName = obj.institutionName;
        this.neispuoStatus = obj.neispuoStatus || new DropDownModel();
        this.absenceCorrection = obj.absenceCorrection;
        this.daysCorrection = obj.daysCorrection;
        this.aspMonthlyBenefitsImportId = obj.aspMonthlyBenefitsImportId;
        this.institutionCode = obj.institutionCode;
        this.createdBy = obj.createdBy;
        this.modifiedBy = obj.modifiedBy;
        this.createdDate = (obj.createdDate && moment) ? moment(obj.createdDate).format(Constants.DATEPICKER_FORMAT) : '';
        this.modifyDate =  (obj.modifyDate && moment) ? moment(obj.modifyDate).format(Constants.DATEPICKER_FORMAT) : '';
        this.basicClassid = obj.basicClassid;
    }
}
