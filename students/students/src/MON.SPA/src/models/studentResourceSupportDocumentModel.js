import { DocumentModel } from './documentModel.js';

export class StudentResourceSupportDocumentModel {

    constructor(obj = {} ) {
        this.id = obj.id || 0;
        this.personId = obj.personId || 0;
        this.description = obj.description || null;
        this.document = obj.document ? obj.document : new DocumentModel();
    }
}