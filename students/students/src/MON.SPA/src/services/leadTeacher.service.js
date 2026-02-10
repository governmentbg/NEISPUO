import http from './http.service';

class LeadTeacherService {
  constructor() {
    this.$http = http;
    this.baseUrl = '/api/leadTeacher';
  }

  getById(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/GetById?id=${id}`);
  }

  getByClassId(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/GetByClassId?id=${id}`);
  }

  list() {
    return this.$http.get(`${this.baseUrl}/list`);
  }

  update(model) {
    if (!model) {
      throw new Error('model is required');
    }

    return this.$http.put(`${this.baseUrl}/Update`, model);
  }

  deleteLeadTeacher(classId) {
    return this.$http.delete(`${this.baseUrl}/Delete?classId=${classId}`);
  }
}

export default new LeadTeacherService();
