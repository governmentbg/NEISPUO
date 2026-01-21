import http from './http.service';
class StudentLodService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/studentLOD";
  }

  getStudentGeneralTrainingDataDetails(studentId, classId) {
    return this.$http.get(`${this.baseUrl}/GetGeneralTrainingDataDetails?studentId=${studentId}&classId=${classId}`);
  }

  getCurriculumDetailsByStudentClass(studentClassId) {
    return this.$http.get(`${this.baseUrl}/GetCurriculumDetailsByStudentClass?studentClassId=${studentClassId}`);
  }

  generatePersonalFile(model) {
    return this.$http.post(`${this.baseUrl}/GeneratePersonalFile`, model, { responseType: 'blob' });
  }

  generatePersonalFileForStay(model) {
    return this.$http.post(`${this.baseUrl}/GeneratePersonalFileForStay`, model,  { responseType: 'blob' });
  }
}

export default new StudentLodService();
