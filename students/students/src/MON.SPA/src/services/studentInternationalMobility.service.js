import http from './http.service';

class StudentInternationalMobility {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/studentInternationalMobility";
  }

  create(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.post(`${this.baseUrl}/Create`, model);
  }

  update(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.put(`${this.baseUrl}/Update`, model);
  }

  getById(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/GetById?internationalMobilityId=${id}`);
  }

  getByPersonId(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/GetByPersonId?personId=${id}`);
  }

  delete(id){

    if (!id) {
        throw new Error('Id is required');
    }

    return this.$http.delete(`${this.baseUrl}/Delete?internationalMobilityId=${id}`);
  }

 
}

export default new StudentInternationalMobility();