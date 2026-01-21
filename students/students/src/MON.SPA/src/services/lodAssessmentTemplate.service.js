import http from './http.service';

class LodAssessmentTemplateService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/LodAssessmentTemplate";
  }

  getById(id){
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

  update(model){
    if (!model) {
        throw new Error('Model is required');
    }

    return this.$http.put( `${this.baseUrl}/Update`, model);
  }

  delete(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.delete(`${this.baseUrl}/Delete?id=${id}`);
  }
}

export default new LodAssessmentTemplateService();
