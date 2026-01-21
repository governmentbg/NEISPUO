import http from './http.service';

class CurriculumService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/curriculum";
  }

  getForStudentClass(studentClassId, statusFilter) {
    if (!studentClassId) {
      throw new Error('StudentClassId is required');
    }

    return this.$http.get(`${this.baseUrl}/GetForStudentClass?studentClassId=${studentClassId}&status=${statusFilter}`);
  }

  removeForStudentClass(model) {
    return this.$http.put(`${this.baseUrl}/removeForStudentClass`, model);
  }


  addForStudentClass(model) {
    return this.$http.put(`${this.baseUrl}/AddForStudentClass`, model);
  }

  editCurriculumStudent(model) {
    return this.$http.put(`${this.baseUrl}/EditCurriculumStudent`, model);
  }
}

export default new CurriculumService();
