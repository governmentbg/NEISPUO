import Constants from '@/common/constants.js';
import { DocumentModel } from '@/models/documentModel.js';

export class StudentSanctionModel {
  constructor(obj = {}, moment = {}) {
    this.id = obj.id || null;
    this.startDate = (obj.startDate && moment) ? moment(obj.startDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.endDate = (obj.endDate && moment) ? moment(obj.endDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.personId = obj.personId || null;
    this.description = obj.description || null;
    this.sanctionTypeId = obj.sanctionTypeId || null;
    this.institutionId = obj.institutionId || null;
    this.cancelOrderDate = (obj.cancelOrderDate && moment) ? moment(obj.cancelOrderDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.cancelOrderNumber = obj.cancelOrderNumber || null;
    this.cancelReason = obj.cancelReason || null;
    this.orderDate = (obj.orderDate && moment) ? moment(obj.orderDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.orderNumber = obj.orderNumber || null;
    this.ruoOrderDate = (obj.ruoOrderDate && moment) ? moment(obj.ruoOrderDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.ruoOrderNumber = obj.ruoOrderNumber || null;
    this.schoolYear = obj.schoolYear;
    this.schoolYearName = obj.schoolYearName;
    this.institutionName = obj.institutionName;
    this.documents = obj.documents ? obj.documents.map(el => new DocumentModel(el)) : [];
  }
}
