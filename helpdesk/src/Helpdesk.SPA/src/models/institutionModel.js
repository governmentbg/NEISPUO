export class InstitutionModel {
  constructor(obj = {}) {
    this.institutionId = obj.instituionId || null;
    this.schoolYear = obj.schoolYear || null;
    this.institutionName = obj.institutionName;
    this.townId = obj.townId || null;
    this.municipalityId = obj.municipalityId || null;
    this.regionId = obj.regionId || null;
  }
}
