import { DocumentModel } from './documentModel.js';
export class IssueCommentModel {

  constructor(obj = {}) {
    this.id = obj.id || null;
    this.issueId = obj.issueId;
    this.comment = obj.comment;
    this.documents = obj.documents ? obj.documents.map(el => new DocumentModel(el)) : [];
  }
}
