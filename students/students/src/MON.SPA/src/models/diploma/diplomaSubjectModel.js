import { BasicDocumentTemplateSubjectModel } from '@/models/diploma/basicDocumentTemplateSubjectModel';

export class DiplomaSubjectModel extends BasicDocumentTemplateSubjectModel {
    constructor(obj = {}) {
      super(obj);
      this.diplomaId = obj.diplomaId;
      this.grade = obj.grade;
      this.gradeText = obj.gradeText;
      this.lockedForEdit = obj.lockedForEdit || undefined;
      this.gradeCategory = obj.gradeCategory;
      this.specialNeedsGrade = obj.specialNeedsGrade;
      this.otherGrade = obj.otherGrade;
      this.qualitativeGrade = obj.qualiatativeGrade;
    }
  }
