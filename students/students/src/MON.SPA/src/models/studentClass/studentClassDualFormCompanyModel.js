import Constants from '@/common/constants.js';
import { uuid } from 'vue-uuid';

export class StudentClassDualFormCompanyModel {
  constructor(obj = {}, moment = {}) {
    this.id = obj.id;
    this.uid = uuid.v4();
    this.uic = obj.uic;
    this.name = obj.name;
    this.email = obj.email;
    this.phone = obj.phone;
    this.district = obj.district;
    this.municipality = obj.municipality;
    this.settlement = obj.settlement;
    this.area = obj.area;
    this.address = obj.address;
    this.url = obj.url;
    this.startDate = (obj.startDate && moment) ? moment(obj.startDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.endDate = (obj.endDate && moment) ? moment(obj.endDate).format(Constants.DATEPICKER_FORMAT) : '';
  }
}
