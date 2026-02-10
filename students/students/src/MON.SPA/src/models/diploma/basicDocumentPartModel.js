export class BasicDocumentPartModel {
  constructor(obj = {}) {
    this.uid = obj.uid || Math.floor((Math.random()) * 0x10000).toString(16);
    this.id = obj.id;
    this.position = obj.position || 1;
    this.name = obj.name;
    this.description = obj.description;
    this.code = obj.code;
    this.isHorariumHidden = obj.isHorariumHidden;
    this.basicClassId = obj.basicClassId;
    this.subjectTypes = obj.subjectTypes;
    this.externalEvaluationTypes = obj.externalEvaluationTypes;
    this.basicDocumentSubjects = obj.basicDocumentSubjects;
    this.printedLines = obj.printedLines;
    this.totalLines = obj.totalLines;
  }
}
