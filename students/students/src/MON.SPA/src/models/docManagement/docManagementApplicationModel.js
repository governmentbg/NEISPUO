import Constants from '@/common/constants.js';
import { DocumentModel } from '@/models/documentModel.js';

export class DocManagementApplicationModel {
  constructor(obj = {}, moment = {}) {
    this.id = obj.id;
    this.parentId = obj.parentId;
    this.campaignId = obj.campaignId;
    this.institutionId = obj.institutionId;
    this.schoolYear = obj.schoolYear;
    this.schoolYearName = obj.schoolYearName;
    this.campaignName = obj.campaignName;
    this.institutionName = obj.institutionName;
    this.basicDocuments = (obj.basicDocuments || []).map(x => new DocManagementApplicationBasicDocumentModel(x));
    this.status = obj.status;
    this.statusName = obj.statusName;
    this.createDate = (obj.createDate && moment) ? moment(obj.createDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.creator = obj.creator;
    this.modifyDate = (obj.modifyDate && moment) ? moment(obj.modifyDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.updater = obj.updater;

    this.campaign = obj.campaign;
    this.attachments = obj.attachments ? obj.attachments.map(el => new DocumentModel(el)) : [];

    this.deliveryNotes = obj.deliveryNotes;
    this.hasEvaluationPermission = obj.hasEvaluationPermission;

    this.isExchangeRequest = obj.isExchangeRequest;
    this.requestedInstitutionId = obj.requestedInstitutionId;
    this.requestedInstitutionName = obj.requestedInstitutionName;
    this.hasApprovePermission = obj.hasApprovePermission;
  }
}

export class DocManagementApplicationBasicDocumentModel {
  constructor(obj = {}) {
    this.id = obj.id;
    this.basicDocumentId = obj.basicDocumentId;
    this.basicDocumentName = obj.basicDocumentName;
    this.number = obj.number || 0; // Default number to 0 if not provided
    this.hasFactoryNumber = obj.hasFactoryNumber || false;
    this.seriesFormat = obj.seriesFormat;
    this.isDuplicate = obj.isDuplicate || false;
    this.deliveredCount = obj.deliveredCount;
    this.deliveredNumbers = obj.deliveredNumbers;
    this.freeDocCount = obj.freeDocCount;
  }
}

export class DocManagementApplicationReturnForCorrectionModel {
  constructor(obj = {}) {
    this.applicationId = obj.applicationId;
    this.parentId = obj.parentId;
    this.description = obj.description;
  }
}
