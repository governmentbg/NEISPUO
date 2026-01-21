import {StudentClassSummaryModel} from '@/models/studentClass/studentClassSummaryModel.js';
export class StudentSummaryModel {
    constructor(obj = {}) {
      this.personId = obj.personId;
      this.fullName = obj.fullName || '';
      this.gender = obj.gender || '';
      this.pin = obj.pin || '';
      this.pinType = obj.pinType || '';
      this.age = obj.age || 0;
      this.toBeDischarged = obj.toBeDischarged || false;
      this.currentClass = new StudentClassSummaryModel(obj.currentClass);
      this.allCurrentClasses = obj.allCurrentClasses || [];
      this.waitingToBeDischarged = obj.waitingToBeDischarged || [];
      this.azureAccountDeleted = obj.azureAccountDeleted;
      this.email = obj.email;
      this.mobilePhone = obj.mobilePhone;
    }
  }
