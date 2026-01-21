export class LodFinalizationStudentModel {
  constructor(obj = {}) {
    this.personId = obj.personId;
    this.fullName = obj.fullName;
    this.isLodApproved = obj.isLodApproved;
    this.isLodFinalized = obj.isLodFinalized;
  }
}
