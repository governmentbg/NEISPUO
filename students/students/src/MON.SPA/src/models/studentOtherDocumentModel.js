import Constants from '@/common/constants.js';
import { DocumentModel } from './documentModel.js';

export class StudentOtherDocumentModel {
    constructor(obj = {} , moment = {}) {
        this.id = obj.id || null;
        this.description = obj.description || null;
        this.series = obj.series || null;
        this.factoryNumber = obj.factoryNumber || null;
        this.regNumberTotal = obj.regNumberTotal || null;
        this.regNumber = obj.regNumber || null;
        this.issueDate = (obj.issueDate && moment) ? moment(obj.issueDate).format(Constants.DATEPICKER_FORMAT) : '';
        this.deliveryDate = this.deliveryDate = (obj.deliveryDate && moment) ? moment(obj.deliveryDate).format(Constants.DATEPICKER_FORMAT) : '';
        this.documents = obj.documents ? obj.documents.map(el => new DocumentModel(el)) : [];
        this.institutionId = obj.institutionId || null;
        this.institutionName = obj.institutionName || '';
        this.documentTypeId = obj.documentTypeId || null;
        this.documentTypeName = obj.documentTypeName || '';
        this.personId = obj.personId || null;
        this.documentTypeName = obj.documentTypeName || null;
        this.schoolYear = obj.schoolYear || null;
        this.schoolYearName = obj.schoolYearName || null;
        this.isLodFinalized = obj.isLodFinalized;

    }
}
