import http from './http.service';
class SelfGovernmentService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/selfGovernment";
  }

  getById(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/GetById?id=${id}`);
  }

  getByPersonId(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/GetByPersonId?personId=${id}`);
  }

  update(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.put(`${this.baseUrl}/Update`, model);
  }

  create(model) {
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
}

export default new SelfGovernmentService();