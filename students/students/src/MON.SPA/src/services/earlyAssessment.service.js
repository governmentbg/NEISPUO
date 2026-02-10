import http from './http.service';

class EarlyAssessmentService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/EarlyAssessment";
  }

  getByPerson(personId){
    if (!personId) {
        throw new Error('PersonId is required');
    }

    return this.$http.get(`${this.baseUrl}/GetByPerson?personId=${personId}`);
  }

  createOrUpdate(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.post(`${this.baseUrl}/AddOrUpdate`, model);
  }


}

export default new EarlyAssessmentService();
