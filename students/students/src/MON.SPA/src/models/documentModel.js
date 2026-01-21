export class DocumentModel {
  constructor(obj = {}) {
    this.id = obj.id || '';
    this.noteFileName = obj.noteFileName || '';
    this.noteFileType = obj.noteFileType || '';
    this.section = obj.section  || '';
    this.description = obj.description  || '';
    this.blobId = obj.blobId || '';
    this.unixTimeSeconds = obj.unixTimeSeconds || 0;
    this.hmac = obj.hmac || '';
    this.blobServiceUrl = obj.blobServiceUrl || '';
    this.deleted = obj.deleted || false;
    this.uid = obj.uid || obj.id;
    this.size = obj.size; // Големина в байта
  }

  get sizeInKb() {
    if (!this.size) {
      return this.size;
    }

    return this.size / 1024;
  }

  get sizeInMb() {
    if (!this.size) {
      return this.size;
    }

    return this.size / 1048576;
  }
}
