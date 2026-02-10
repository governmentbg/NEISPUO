import Constants from '@/common/constants.js';

export class DiplomaFinalizationModel {
  constructor(obj = {}, moment = {}) {
    this.signedDate = (obj.signedDate && moment) ? moment(obj.signedDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.finalizedDate = (obj.finalizedDate && moment) ? moment(obj.finalizedDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.step = obj.currentStepNumber || 0;
    this.isPublic = obj.isPublic || false;
    this.isSigned = obj.isSigned || false;
    this.isFinalized = obj.isFinalized || false;
  }
}