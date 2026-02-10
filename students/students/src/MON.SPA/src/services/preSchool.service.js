import http from './http.service';
import QueryString from "query-string";

class PreSchoolService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/preSchoolEvaluation";
  }

  getByPersonId(personId) {
    return this.$http.get(`${this.baseUrl}/GetByPersonId?personId=${personId}`);
  }

  getEvalById(id) {
    return this.$http.get(`${this.baseUrl}/GetEvalById?id=${id}`);
  }

  getReadinessForFirstGrade(personId) {
    return this.$http.get(`${this.baseUrl}/GetReadinessForFirstGrade?personId=${personId}`);
  }

  createForBaiscClass(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.post(`${this.baseUrl}/CreateForBasicClass`, model);
  }

  createReadinessForFirstGrade(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.post(`${this.baseUrl}/CreateReadinessForFirstGrade`, model);
  }

  update(evaluation) {
    return this.$http.put(`${this.baseUrl}/Update`, evaluation);
  }


  importFromSchoolBook(personId, basicClassId, schoolYear) {
    const params = {};
    if (personId) params.personId = personId;
    if (basicClassId) params.basicClassId = basicClassId;
    if (schoolYear) params.schoolYear = schoolYear;

    return this.$http.get(`${this.baseUrl}/ImportFromSchoolBook?${QueryString.stringify(params)}`);
  }


  updateReadinessForFirstGrade(readiness) {
    return this.$http.put(`${this.baseUrl}/UpdateReadinessForFirstGrade`, readiness);
  }

  delete(id) {
    return this.$http.delete(`${this.baseUrl}/Delete?id=${id}`);
  }

  deleteReadiness(id) {
    return this.$http.delete(`${this.baseUrl}/DeleteReadiness?id=${id}`);
  }

}

export default new PreSchoolService();
