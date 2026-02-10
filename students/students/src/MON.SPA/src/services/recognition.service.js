import http from './http.service';

class RecognitionService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/recognition";
  }

  getById(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/GetById?id=${id}`);
  }

  getListForPerson(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/GetListForPerson?personId=${id}`);
  }

  create(model) {
    if (!model) {
      throw new Error('RecognitionModel model is required');
    }

    return this.$http.post(`${this.baseUrl}/Create`, model);
  }

  update(model) {
    if (!model) {
      throw new Error('RecognitionModel model is required');
    }

    return this.$http.put(`${this.baseUrl}/Update`, model);
  }

  delete(id) {
    return this.$http.delete(`${this.baseUrl}/Delete?id=${id}`);
  }

  getRecognitionRequiredSubjects() {
    return this.$http.get(`${this.baseUrl}/GetRecognitionRequiredSubjects`);
  }
}

export default new RecognitionService();