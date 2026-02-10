export class SchoolTypeLodAccessModel{
  constructor(obj = {}){ 
    this.detailedSchoolTypeId = obj.detailedSchoolTypeId || undefined;
    this.detailedSchoolTypeName = obj.detailedSchoolTypeName || '';
    this.isLodAccessAllowed = obj.isLodAccessAllowed || false;
  }
}