import Constants from '@/common/constants.js';

export class AbsenceCampaignModel {
  constructor(obj = {}, moment = {}) {
    this.id = obj.id || null;
    this.name = obj.name;
    this.description = obj.description;
    this.schoolYear = obj.schoolYear;
    this.month = obj.month;
    this.isManuallyActivated = obj.isManuallyActivated;
    this.active = obj.active;
    this.fromDate = (obj.fromDate && moment) ? moment(obj.fromDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.toDate = (obj.toDate && moment) ? moment(obj.toDate).format(Constants.DATEPICKER_FORMAT) : '';
  }
}
