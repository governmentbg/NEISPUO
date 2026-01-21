export class DocumentModel {
  constructor(obj = {}) {
    this.id = obj.id || null;
    this.noteFileName = obj.noteFileName || '';
    this.noteFileType = obj.noteFileType || '';
    this.section = obj.section || '';
    this.description = obj.description || '';
    this.blobId = obj.blobId || null;
    this.unixTimeSeconds = obj.unixTimeSeconds || 0;
    this.hmac = obj.hmac || '';
    this.blobServiceUrl = obj.blobServiceUrl || '';
    this.deleted = obj.deleted || false;
    this.uid = obj.uid || obj.id;
  }
}
