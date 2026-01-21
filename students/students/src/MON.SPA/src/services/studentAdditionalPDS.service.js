import http from './http.service';

class StudentAdditionalPDSService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/AdditionalPersonalDevelopmentSupport";
  }

  getById(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/GetById?id=${id}`);
  }

  getSopForPerson(personId, schoolYear) {
    return this.$http.get(`${this.baseUrl}/getSopForPerson?personId=${personId}&schoolYear=${schoolYear}`);
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

  delete(id) {
    return this.$http.delete(`${this.baseUrl}/Delete?id=${id}`);
  }

  suspendAdditionalPersonalDevelopmentSupport(model) {
    if (!model) {
      throw new Error('Suspend model is required');
    }

    return this.$http.put(`${this.baseUrl}/SuspendAdditionalPersonalDevelopmentSupport`, model);
  }
}

export default new StudentAdditionalPDSService();
