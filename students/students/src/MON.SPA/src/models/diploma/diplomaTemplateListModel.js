export class DiplomaTemplateListModel {
    constructor(obj = {}) {
      this.id = obj.id || null;
      this.name = obj.name || null;
      this.institutionName = obj.institutionName || null;
      this.basicDocumentTypeName = obj.basicDocumentTypeName || null;
      this.canBeDeleted = obj.canBeDeleted || null;
    }
  }