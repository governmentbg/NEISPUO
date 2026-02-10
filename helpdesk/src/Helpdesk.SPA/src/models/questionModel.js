export class QuestionModel {
  constructor(obj = {}) {
    this.id = obj.id || null;
    this.question = obj.question;
    this.answer = obj.answer || '';
    this.createDate = obj.createDate || null;
    this.modifyDate = obj.modifyDate || null;
  }
}
