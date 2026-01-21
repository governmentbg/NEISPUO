import Constants from '@/common/constants.js';
import { DocumentModel } from '@/models/documentModel.js';

export class DocManagementCampaignModel {
  constructor(obj = {}, moment = {}) {
    this.id = obj.id || null;
    this.name = obj.name;
    this.description = obj.description;
    this.schoolYear = obj.schoolYear;
    this.isManuallyActivated = obj.isManuallyActivated;
    this.active = obj.active;
    this.fromDate = (obj.fromDate && moment) ? moment(obj.fromDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.toDate = (obj.toDate && moment) ? moment(obj.toDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.schoolYearName = obj.schoolYearName;
    this.institutionId = obj.institutionId;
    this.institutionName = obj.institutionName;
    this.parentId = obj.parentId;
    this.labels = obj.labels || [];
    this.attachments = obj.attachments ? obj.attachments.map(el => new DocumentModel(el)) : [];
    this.hasRuoAttachmentPermision = obj.hasRuoAttachmentPermision;
  }
}
