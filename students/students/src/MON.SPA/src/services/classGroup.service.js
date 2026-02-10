import http from './http.service';

class ClassGroupService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/ClassGroup";
  }

  getById(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/GetById?id=${id}`);
  }

  getStudents(classId) {
    if (!classId) {
      throw new Error('ClassId is required');
    }

    return this.$http.get(`${this.baseUrl}/GetStudents?classId=${classId}`);
  }

  getStudentsForEnrollment(classId) {
    if (!classId) {
      throw new Error('ClassId is required');
    }

    return this.$http.get(`${this.baseUrl}/GetStudentsForEnrollment?classId=${classId}`);
  }

  getStudentsForMassEnrollment(classId) {
    if (!classId) {
      throw new Error('ClassId is required');
    }

    return this.$http.get(`${this.baseUrl}/GetStudentsForMassEnrollment?classId=${classId}`);
  }

}

export default new ClassGroupService();
