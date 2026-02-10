export class StudentSelfGovernmentModel {
  constructor(obj = {}) {
    this.id = obj.id || null;
    this.schoolYear = obj.schoolYear || null;
    this.personId = obj.personId || null;
    this.participationId = obj.participationId || null;
    this.participationName = obj.participationName;
    this.positionId = obj.positionId || null;
    this.positionName = obj.positionName;
    this.institutionId = obj.institutionId || null;
    this.institutionName = obj.institutionName;
    this.participationAdditionalInformation = obj.participationAdditionalInformation;
    this.additionalInformation = obj.additionalInformation;
    this.mobilePhone = obj.mobilePhone;
    this.email = obj.email;
    this.institution = obj.institution;
    this.isLodFinalized = obj.isLodFinalized;
    this.schoolYearName = obj.schoolYearName;
  }
}
