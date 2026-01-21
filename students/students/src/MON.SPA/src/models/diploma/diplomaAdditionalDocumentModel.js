import Constants from '@/common/constants.js';
import moment from 'moment';

export class DiplomaAdditionalDocumentModel {
  constructor(obj = {}) {
    this.uid = obj.uid || Math.floor((Math.random()) * 0x10000).toString(16);
    this.id = obj.id;
    this.mainDiplomaId = obj.mainDiplomaId;
    this.basicDocumentId = obj.basicDocumentId;
    this.institutionId = obj.institutionId;
    this.institutionName = obj.institutionName;
    this.institutionAddress = obj.institutionAddress;
    this.institutionDetails = obj.institutionDetails;
    this.town = obj.town || null;
    this.municipality = obj.municipality || null;
    this.region = obj.region || null;
    this.localArea = obj.localArea || null;
    this.series = obj.series || null;
    this.factoryNumber = obj.factoryNumber;
    this.registrationNumber = obj.registrationNumber;
    this.registrationNumberYear = obj.registrationNumberYear;
    this.registrationDate = obj.registrationDate ? moment(obj.registrationDate).format(Constants.DATEPICKER_FORMAT) : obj.registrationDate;
  }
}
