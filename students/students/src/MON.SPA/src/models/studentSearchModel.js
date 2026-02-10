export class StudentSearchModel {
    constructor(obj = {}) {
      this.pin = obj.pin || '';
      this.firstName = obj.firstName || '';
      this.middleName = obj.middleName || '';
      this.lastName = obj.lastName || '';
      this.publicEduNumber = obj.publicEduNumber || '';
      this.district = obj.district || '';
      this.municipality = obj.municipality || '';
      this.school = obj.school || '';
      this.exactMatch = obj.exactMatch || true;
      this.filter = obj.filter || '';
      this.pageIndex = obj.pageIndex || 0;
      this.pageSize = obj.pageSize || 10;
      this.sortBy = obj.sortBy || '';
      this.onlyOwnInstitution = obj.onlyOwnInstitution || false;
    }
  }