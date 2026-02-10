import http from './http.service';

class ExternalEvaluationService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/externalEvaluation";
  }

  
  getByPersonId(personId) {
    if(!personId) {
      throw new Error('PersonId is required');
    }
    
    return this.$http.get(`${this.baseUrl}/GetByPersonId?personId=${personId}`);
  }
  
  create(model) {
    if (!model) {
      throw new Error('External evaluation model is required');
    }
    
    return this.$http.post(`${this.baseUrl}/Create`, model);
  }
  
  update(model) {
    if (!model) {
      throw new Error('External evaluation model is required');
    }
    
    return this.$http.put(`${this.baseUrl}/Update`, model);
  }
  
  delete(id) {
    if (!id) {
      throw new Error('External evaluation id is required');
    }
    
    return this.$http.delete(`${this.baseUrl}/Delete?id=${id}`);
  }

  getExternalEvaluationTypes() {
    return this.$http.get(`${this.baseUrl}/getExternalEvaluationTypes`);
  }
}

export default new ExternalEvaluationService();