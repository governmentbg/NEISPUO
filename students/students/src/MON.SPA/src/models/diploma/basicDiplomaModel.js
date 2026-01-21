export class BasicDiplomaModel {
  constructor(obj = {}) {
    this.personalIdType = obj.pinType && obj.pinType.value.toString() || null;
    this.personalId = obj.pin || null;
    this.firstName = obj.firstName || null;
    this.middleName = obj.middleName || null;
    this.lastName = obj.lastName || null;
    this.gender = obj.gender || null;
    this.birthDate = obj.birthDate || null;
    this.personId = obj.id || null;
  }
}