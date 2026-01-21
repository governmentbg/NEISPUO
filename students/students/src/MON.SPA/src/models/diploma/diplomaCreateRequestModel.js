import Constants from '@/common/constants.js';

export class DiplomaCreateRequestModel {
  constructor(obj = {}, moment = {}) {
    this.id = obj.id;
    this.personId = obj.personId;
    this.requestingInstitutionId = obj.requestingInstitutionId;
    this.currentInstitutionId = obj.currentInstitutionId;
    this.basicDocumentId = obj.basicDocumentId;
    this.registrationNumber = obj.registrationNumber;
    this.registrationNumberYear = obj.registrationNumberYear;
    this.registrationDate = (obj.registrationDate && moment) ? moment(obj.registrationDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.note = obj.note;
    this.isGranted = obj.isGranted;
    this.arbitraryCurrentInstitutionName = obj.arbitraryCurrentInstitutionName;
  }
}
