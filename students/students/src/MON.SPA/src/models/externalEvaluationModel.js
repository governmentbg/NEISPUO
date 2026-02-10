export class ExternalEvaluationModel {
  constructor(obj = {}) {
      this.id = obj.id || '';
      this.parentId = obj.parentId || '';
      this.typeId = obj.typeId || '';
      this.type = obj.type || '';
      this.schoolYear = obj.schoolYear || '';
      this.personId = obj.personId || '';
      this.evaluations = obj.evaluations || [];
      this.uid = obj.uid || '';
  }
}