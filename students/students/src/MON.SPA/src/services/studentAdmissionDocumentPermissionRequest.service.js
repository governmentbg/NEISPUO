import http from './http.service';

class StudentAdmissionDocumentPermissionRequestService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/admissionPermissionRequest";
  }

  getById(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/GetById?id=${id}`);
  }

  create(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.post(`${this.baseUrl}/Create`, model);
  }

  update(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.put(`${this.baseUrl}/Update`, model);
  }

  delete(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.delete(`${this.baseUrl}/Delete?id=${id}`);
  }

  confirm(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.put(`${this.baseUrl}/Confirm?id=${id}`);
  }

  countPending() {
    return this.$http.get(`${this.baseUrl}/CountPending`);
  }
}

export default new StudentAdmissionDocumentPermissionRequestService();