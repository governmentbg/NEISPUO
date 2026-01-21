export class PrintTemplateModel {
  constructor(obj = {}) {
    this.id = obj.id || null;
    this.basicDocumentId = obj.basicDocumentId;
    this.basicDocumentName = obj.basicDocumentName;
    this.printFormId = obj.printFormId,
    this.printFormEdition = obj.printFormEdition,
    this.name = obj.name || null;
    this.description = obj.description || null;
    this.institutionId = obj.institutionId || null;
    this.hasContents = obj.hasContents || false;
    this.left1Margin = obj.left1Margin || 0;
    this.top1Margin = obj.top1Margin || 0;
    this.left2Margin = obj.left2Margin || 0;
    this.top2Margin = obj.top2Margin || 0;
  }
}
