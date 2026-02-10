import Constants from '@/common/constants.js';
import { DocumentModel } from './documentModel.js';

export class RecognitionModel {
  constructor(obj = {}, moment = {}) {
    this.id = obj.id || null;
    this.personId = obj.personId || null;
    this.institutionName = obj.institutionName || null;
    this.institutionCountryId = obj.institutionCountryId || null;
    this.educationLevelId = obj.educationLevelId || null;
    this.sppooProfessionId = obj.sppooProfessionId || null;
    this.sppooSpecialityId = obj.sppooSpecialityId || null;
    this.term = obj.term || null;
    this.basicClassId = obj.basicClassId || null;
    this.diplomaNumber = obj.diplomaNumber || null;
    this.diplomaDate = (obj.diplomaDate && moment) ? moment(obj.diplomaDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.ruoDocumentNumber = obj.ruoDocumentNumber || null;
    this.ruoDocumentDate = (obj.ruoDocumentDate && moment) ? moment(obj.ruoDocumentDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.documents = obj.documents ? obj.documents.map(el => new DocumentModel(el)) : [];
    this.equalizations = obj.equalizations || [];
    this.isSelfEduForm = obj.isSelfEduForm || false;

    // From the view model
    this.institutionCountry = obj.institutionCountry || '';
    this.basicClass = obj.basicClass || '';
    this.termName = obj.termName || '';
    this.educationLevel = obj.educationLevel || '';
    this.sppooProfession = obj.sppooProfession || '';
    this.sppooSpeciality = obj.sppooSpeciality || '';
    this.schoolYear = obj.schoolYear;
    this.schoolYearName = obj.schoolYearName;
    this.basicClassName = obj.basicClassName;
    this.educationLeveName = obj.educationLeveName;
    this.sppooProfessionName = obj.sppooProfessionName;
    this.sppooSpecialityName = obj.sppooSpecialityName;
    this.vetLevel = obj.vetLevel;
  }
}
