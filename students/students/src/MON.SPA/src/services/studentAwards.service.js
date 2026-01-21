import http from './http.service';

class StudentAwardsService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/studentAwards";
  }

  getById(id){
    if (!id) {
        throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/GetById?id=${id}`);
  }

  getByPersonId(personId) {
    if (!personId) {
      throw new Error('PersonId is required');
    }

    return this.$http.get(`${this.baseUrl}/GetByPersonId?personId=${personId}`);
  }

  delete(id) {
    return this.$http.delete(`${this.baseUrl}/Delete?id=${id}`);
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
}

export default new StudentAwardsService();