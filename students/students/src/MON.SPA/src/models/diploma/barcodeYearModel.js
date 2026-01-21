export class BarcodeYearModel {
    constructor(obj = {}) {
      this.id = obj.id || 0;
      this.edition = obj.edition || null;
      this.schoolYear = obj.schoolYear || null;
      this.headerPage = obj.headerPage || null;
      this.internalPage = obj.internalPage || null;
      this.basicDocumentId = obj.basicDocumentId || null;
    }
  }