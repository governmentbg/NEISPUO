import http from './http.service';

class SchoolTypeLodAccessService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/schoolTypeLodAccess";
  }

  getAll() {
    return this.$http.get(`${this.baseUrl}/GetAll`);
  }

  getById(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/GetById/?detailedSchoolTypeId=${id}`);
  }

  update(model) {
    if (!model) {
      throw new Error('SchoolTypeLodAccess model is required');
    }

    return this.$http.put(`${this.baseUrl}/Update`, model);
  }
}

export default new SchoolTypeLodAccessService();