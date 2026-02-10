import { DropDownModel } from "../dropdownModel";

export class SubjectDetailsDropDownModel extends DropDownModel {
  constructor(obj = {}) {
    super(obj);
    this.subjectTypeId = obj.subjectTypeId;
    this.partId = obj.partId;
  }
}