import Constants from '@/common/constants.js';

export class StudentInternationalProtection {
  constructor(obj = {}, moment = {}) {
    this.id = obj.id;
    this.protectionStatus = obj.protectionStatus;
    this.docNumber = obj.docNumber || '';
    this.docDate = (obj.docDate && moment) ? moment(obj.docDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.validFrom = (obj.validFrom && moment) ? moment(obj.validFrom).format(Constants.DATEPICKER_FORMAT) : '';
    this.validTo = (obj.validTo && moment) ? moment(obj.validTo).format(Constants.DATEPICKER_FORMAT) : '';
  }
}
