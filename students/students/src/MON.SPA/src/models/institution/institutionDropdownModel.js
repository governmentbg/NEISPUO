import { DropDownModel } from "../dropdownModel";

export class InstitutionDropDownModel extends DropDownModel {
  constructor(obj = {}) {
    super(obj);
    this.baseSchoolTypeId = obj.baseSchoolTypeId;
  }
}
  