import http from './http.service';

class DiplomaValidationExclusionService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/diplomaImportValidationExclusion";
  }

  getList() {
    return this.$http.get(`${this.baseUrl}/list`);
  }

  addOrUpdate(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.post(`${this.baseUrl}/AddOrUpdate`, model);
  }

  delete(id) {
    return this.$http.delete(`${this.baseUrl}/Delete?id=${id}`);
  }
}

export default new DiplomaValidationExclusionService();
