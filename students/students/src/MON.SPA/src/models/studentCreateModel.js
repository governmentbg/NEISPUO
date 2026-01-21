export class StudentModel {
  constructor(obj = {}) {
    this.id = obj.id || null;
    this.pin = obj.pin || '';
    this.publicEduNumber = obj.publicEduNumber || '';
    this.pinType = obj.pinType || null;
    this.firstName = obj.firstName || '';
    this.middleName = obj.middleName || '';
    this.lastName = obj.lastName || '';
    this.birthDate = obj.birthDate || '';
    this.gender = obj.gender && obj.gender.value || '';
    this.nationalityId = obj.nationalityId;
    this.birthPlaceCountryId = obj.birthPlaceCountryId;
    this.birthPlaceId = obj.birthPlaceId;
    this.permanentResidenceId = obj.permanentResidenceId;
    this.usualResidenceId = obj.usualResidenceId;
    this.currentAddress = obj.currentAddress || '';
    this.permanentAddress = obj.permanentAddress || '';
    this.phoneNumber = obj.phoneNumber || '';
    this.email = obj.email || '';
    this.addressCoincidesCheck = obj.addressCoincidesCheck || false;
    this.internationalProtectionStatus = obj.internationalProtectionStatus || false;
    this.hasIndividualStudyPlan = obj.hasIndividualStudyPlan || false;
    this.hasSupportiveEnvironment = obj.hasSupportiveEnvironment || false;
    this.supportiveEnvironment = obj.supportiveEnvironment || '';
    this.repeater = obj.repeater || '';
    this.birthPlace = obj.birthPlace || null;
    this.genderId = obj.genderId;
  }
}
