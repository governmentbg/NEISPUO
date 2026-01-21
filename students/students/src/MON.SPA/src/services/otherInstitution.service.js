import http from './http.service';

class OtherInstitutionService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/otherInstitution";
  }

  getStudentOtherInstitutions(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/GetStudentOtherInstitutions?personId=${id}`);
  }

  create(model){
    if (!model) {
        throw new Error('Model is required');
      }

      return this.$http.post(`${this.baseUrl}/Create`, model);
  }

  delete(id){
    if (!id) {
        throw new Error('Id is required');
    }

    return this.$http.delete(`${this.baseUrl}/Delete?id=${id}`);
  }

  getById(id){
    if (!id) {
        throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/GetById?institutionId=${id}`);
  }

  update(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.put(`${this.baseUrl}/Update`, model);
  }
}
  
export default new OtherInstitutionService();