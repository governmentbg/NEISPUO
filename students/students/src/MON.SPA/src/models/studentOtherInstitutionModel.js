import Constants from '@/common/constants.js';
import { DropDownModel } from './dropdownModel.js';

export class StudentOtherInstitutionModel {
  constructor(obj = {}, moment = {}) {
    this.id = obj.id || 0;
    this.reason = obj.reason || '';
    this.personId = obj.personId || '';
    this.institution = obj.institution || new DropDownModel();
    this.validFrom = (obj.validFrom && moment) ? moment(obj.validFrom).format(Constants.DATEPICKER_FORMAT) : '';
    this.validTo = (obj.validTo && moment) ? moment(obj.validTo).format(Constants.DATEPICKER_FORMAT) : '';
  }
}
