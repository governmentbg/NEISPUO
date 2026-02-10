import Constants from '@/common/constants.js';
import { DocumentModel } from '@/models/documentModel.js';

export class StudentScholarshipModel {
  constructor(obj = {}, moment = {}) {
    this.id = obj.id || null;
    this.personId = obj.personId;
    this.scholarshipTypeId = obj.scholarshipTypeId;
    this.scholarshipTypeName = obj.scholarshipTypeName;
    this.scholarshipAmountId = obj.scholarshipAmountId;
    this.scholarshipAmountName = obj.scholarshipAmountName;
    this.amountRate = obj.amountRate || null;
    this.description = obj.description;
    this.periodicity = obj.periodicity || null;
    this.orderNumber = obj.orderNumber || null;
    this.schoolYear = obj.schoolYear || null;
    this.schoolYearName = obj.schoolYearName;
    this.institutionId = obj.institutionId;
    this.institutionName = obj.institutionName;
    this.scholarshipFinancingOrganId = obj.scholarshipFinancingOrganId;
    this.scholarshipFinancingOrganName = obj.scholarshipFinancingOrganName;
    this.orderDate = (obj.orderDate && moment) ? moment(obj.orderDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.commissionDate =  (obj.commissionDate && moment) ? moment(obj.commissionDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.commissionNumber = obj.commissionNumber || null;
    this.startingDateOfReceiving = (obj.startingDateOfReceiving && moment) ? moment(obj.startingDateOfReceiving).format(Constants.DATEPICKER_FORMAT) : '';
    this.endDateOfReceiving = (obj.endDateOfReceiving && moment) ? moment(obj.endDateOfReceiving).format(Constants.DATEPICKER_FORMAT) : '';
    this.documents = obj.documents ? obj.documents.map(el => new DocumentModel(el)) : [];
    this.isLodFinalized = obj.isLodFinalized;
    this.currency = obj.currency;
    this.altCurrency = obj.altCurrency;
    this.altAmountRate = obj.altAmountRate;
    this.amountRateStr = obj.amountRateStr;
  }
}
