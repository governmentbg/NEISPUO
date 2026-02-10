export class ExternalEvaluationItemModel {
  constructor(obj = {}) {
      this.id = obj.id || '';
      this.subject = obj.subject || '';
      this.points = obj.points || '';
      this.description = obj.description || '';
  }
}