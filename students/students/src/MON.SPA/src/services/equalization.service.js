import http from './http.service';

class EqualizationService {
    constructor() {
      this.$http = http;
      this.baseUrl = "/api/equalization";
    }
  create(model) {
    if (!model){
      throw new Error('EqualizationModel model is required');
    }

    return this.$http.post(`${this.baseUrl}/Create`, model);
  }
  getListForPerson(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/GetListForPerson?personId=${id}`);
  }

  getById(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/GetById?id=${id}`);
  }

  update(model) {
    if (!model) {
      throw new Error('Equalization model is required');
    }

    return this.$http.put(`${this.baseUrl}/Update`, model);
  }

  delete(id) {
    return this.$http.delete(`${this.baseUrl}/Delete?id=${id}`);
  }
}

export default new EqualizationService();
