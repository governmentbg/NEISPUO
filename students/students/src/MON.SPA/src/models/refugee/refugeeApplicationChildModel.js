import Constants from '@/common/constants.js';

export class RefugeeApplicationChildModel{
  constructor(obj = {}, moment = {}) {
    this.uid = obj.uid || Math.floor((Math.random()) * 0x10000).toString(16);
    this.id = obj.id,
    this.personId = obj.personId,
    this.firstName = obj.firstName;
    this.middleName = obj.middleName;
    this.lastName = obj.lastName;
    this.birthDate = (obj.birthDate && moment) ? moment(obj.birthDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.nationalityId = obj.nationalityId;
    this.genderId = obj.genderId;
    this.protectionStatus = obj.protectionStatus;
    this.townId = obj.townId;
    this.address = obj.address;
    this.lastInstitutionCountry = obj.lastInstitutionCountry;
    this.ruoDocNumber = obj.ruoDocNumber;
    this.ruoDocDate = (obj.ruoDocDate && moment) ? moment(obj.ruoDocDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.bgLanguageSkill = obj.bgLanguageSkill;
    this.enLanguageSkill = obj.enLanguageSkill;
    this.deLanguageSkill = obj.deLanguageSkill;
    this.frLanguageSkill = obj.frLanguageSkill;
    this.otherLanguageSkill = obj.otherLanguageSkill;
    this.otherLanguage = obj.otherLanguage;
    this.lastInstitutionType = obj.lastInstitutionType;
    this.lastBasicClassId = obj.lastBasicClassId;
    this.isClassCompleted = obj.isClassCompleted;
    this.hasNeedForTextbooks = obj.hasNeedForTextbooks || false;
    this.hasNeedForResourceSupport = obj.hasNeedForResourceSupport || false;
    this.personalId = obj.personalId;
    this.personalIdType = obj.personalIdType == undefined || obj.personalIdType == null ? 1 : obj.personalIdType;
    this.institutionId = obj.institutionId;
    this.gender = obj.gender;
    this.email = obj.email;
    this.phone = obj.phone;
    this.profession = obj.profession;
    this.protectionStatus = obj.protectionStatus;
    this.hasDualCitizenship = obj.hasDualCitizenship || false;
    this.hasDocumentForCompletedClass = obj.hasDocumentForCompletedClass || false;

    // from view model
    this.personalIdTypeName = obj.personalIdTypeName;
    this.nationality = obj.nationality;
    this.genderName = obj.genderName;
    this.town = obj.town;
    this.institution = obj.institution;
    this.lastInstitutionCountryName = obj.lastInstitutionCountryName;
    this.lastBasicClassName = obj.lastBasicClassName;
    this.status = obj.status;
    this.statusName = obj.statusName;
    this.cancellationReason = obj.cancellationReason;
    this.hasValidRouOrderAttrs = obj.hasValidRouOrderAttrs;
    this.canBeDeleted = obj.canBeDeleted;
    this.canBeCancelled = obj.canBeCancelled;
    this.canBeEdited = obj.canBeEdited;
    this.canBeCompleted = obj.canBeCompleted;
    this.canBeSetAsEditable = obj.canBeSetAsEditable;
  }
}
