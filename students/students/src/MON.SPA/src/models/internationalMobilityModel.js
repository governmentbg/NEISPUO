import Constants from '@/common/constants.js';
import { DocumentModel } from '@/models/documentModel.js';

export class InternationalMobilityModel {
    constructor(obj = {}, moment = {}) {
        this.id = obj.id || null;
        this.project = obj.project || null;
        this.receivingInstitution = obj.receivingInstitution || null;
        this.mainObjectives = obj.mainObjectives || null;
        this.countryId = obj.countryId;
        this.fromDate  = (obj.fromDate && moment) ? moment(obj.fromDate).format(Constants.DATEPICKER_FORMAT) : '';
        this.toDate  = (obj.toDate && moment) ? moment(obj.toDate).format(Constants.DATEPICKER_FORMAT) : '';
        this.personId = obj.personId || null;
        this.institutionId = obj.institutionId;
        this.schoolYear = obj.schoolYear;
        this.schoolYearName = obj.schoolYearName;
        this.documents = obj.documents ? obj.documents.map(el => new DocumentModel(el)) : [];
    }
}
