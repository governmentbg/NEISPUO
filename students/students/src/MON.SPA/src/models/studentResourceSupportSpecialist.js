export class StudentResourceSupportSpecialist {
  constructor(obj = {}) {
    this.id = obj.id || 0;
    this.name = obj.name || '';
    this.organizationName = obj.organizationName || '';
    this.resourceSupportId = obj.resourceSupportId || 0;
    this.workPlaceId = obj.workPlaceId || ''; 
    this.resourceSupportSpecialistTypeId = obj.resourceSupportSpecialistTypeId || '';
    this.organizationType = obj.organizationType || '';
    this.specialistType = obj.specialistType || '';
    this.sysUserID = obj.sysUserID || '';
  }
}
