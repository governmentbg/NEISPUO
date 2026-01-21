import Constants from '@/common/constants.js';

export class MonthlyBenefitsViewModel {
    constructor(obj = {},  moment = {}) {
        this.importeBenefitsDetails = obj.importeBenefitsDetails;
        this.totalRecords = obj.totalRecords;
        this.month = obj.month;
        this.schoolYear = obj.schoolYear;
        this.dateImported = obj.dateImported;
        this.recordsCount = obj.recordsCount;
        this.isSigned = obj.isSigned;
        this.signedDate = obj.signedDate;
        this.isActive = obj.isActive;
        this.fromDate = (obj.fromDate && moment) ? moment(obj.fromDate).format(Constants.DATEPICKER_FORMAT) : '';
        this.toDate = (obj.toDate && moment) ? moment(obj.toDate).format(Constants.DATEPICKER_FORMAT) : '';
        this.aspConfirmSessionCount = obj.aspConfirmSessionCount;
        this.monConfirmSessionCount = obj.monConfirmSessionCount;
    }
}
