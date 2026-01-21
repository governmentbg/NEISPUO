import http from './http.service';
import QueryString from "query-string";

class LodAssessmentService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/LodAssessment";
  }

  getPersonAssessments(personId, basicClass, schoolYear, isSelfEduForm, filterForCurrentInstitution, filterForCurrentSchoolBook, abortController) {
    const params = {
      personId,
      basicClass,
      schoolYear,
      isSelfEduForm,
      filterForCurrentInstitution,
      filterForCurrentSchoolBook
    };

    if(!abortController) abortController = new AbortController();

    return this.$http.get(`${this.baseUrl}/GetPersonAssessments?${QueryString.stringify(params)}`, {
      signal: abortController.signal
    });
  }

  import(formData) {
    if (!formData) {
      throw new Error('You must choose a file!');
    }

    return this.$http.post(`${this.baseUrl}/import`, formData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
  }

  exportFile(model) {
    if (!model){
      throw new Error('Model is required');
    }

    return this.$http.post(`${this.baseUrl}/ExportFile`, model);
  }

  validateImport(formData) {
    if (!formData) {
      throw new Error('You must choose a file!');
    }

    return this.$http.post(`${this.baseUrl}/validateImport`, formData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
  }

  createOrUpdate(model) {
    if (!model){
      throw new Error('Model is required');
    }

    return this.$http.post(`${this.baseUrl}/CreateOrUpdate`, model);
  }

  getMainStudentClasses(personId) {
    if (!personId){
      throw new Error('PersonId is required');
    }
    return this.$http.get(`${this.baseUrl}/GetMainStudentClasses?personId=${personId}`);
  }

  getStudentClassCurriculum(studentClassId) {
    if (!studentClassId){
      throw new Error('StudentClassId is required');
    }
    return this.$http.get(`${this.baseUrl}/GetStudentClassCurriculum?studentClassId=${studentClassId}`);
  }

  delete(params) {
    if (!params){
      throw new Error('Params is required');
    }

    return this.$http.delete(`${this.baseUrl}/Delete?${QueryString.stringify(params)}`);
  }
}

export default new LodAssessmentService();
