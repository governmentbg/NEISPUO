export class LodFinalizationModel {
  constructor(obj = {}) {
    this.personIds = obj.personIds || [];
    this.schoolYear = obj.schoolYear;
  }
}