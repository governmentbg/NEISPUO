export class DropDownModel {
  constructor(obj = {}) {
    this.value = obj.value || '';
    this.text = obj.text || '';
    this.name = obj.name || '';
    this.relatedObjectId = obj.relatedObjectId || undefined;
  }
}
