export class SearchModel {
  constructor(obj = {}) {
    this.idType = obj.idType || 1; // 1 - ЕГН/ЛНЧ, 2 - ЛИН
    this.idNumber = obj.idNumber || '';
    this.docNumber = obj.docNumber || '';
  }
}