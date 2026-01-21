import { DropDownModel } from '@/models/dropdownModel';

export class StudentRelativeModel {
    constructor(obj = {}) {
      this.id = obj.id || 0;
      this.relativeType = obj.relativeType || null;
      this.notes = obj.notes || '';
      this.workStatus = obj.workStatus || null;
      this.description = obj.description || '';
      this.firstName = obj.firstName || '';
      this.middleName = obj.middleName || '';
      this.lastName = obj.lastName || '';
      this.address = obj.address || '';
      this.pinType = obj.pinType || new DropDownModel(); //ЕГН
      this.pin = obj.pin || '';
      this.index = obj.index || '';
      this.email = obj.email || '';
      this.phoneNumber = obj.phoneNumber || '';
      this.educationType = obj.educationType || null;
    }
  }