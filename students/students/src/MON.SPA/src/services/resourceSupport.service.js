import http from './http.service';

class ResourceSupportService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/ResourceSupport";
  }

  getById(id){
    if (!id) {
        throw new Error('sanctionId is required');
    }

    return this.$http.get(`${this.baseUrl}/GetById?id=${id}`);
  }

  getByPersonId(personId) {
    if (!personId) {
      throw new Error('PersonId is required');
    }
    return this.$http.get(`${this.baseUrl}/GetByPersonId?personId=${personId}`);
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

  updateStudentResourceSupport(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.put(`${this.baseUrl}/UpdateStudentResourceSupport`, model);
  }

  getStudentResourceSupport(id, schoolYear) {
    if (!id) {
      throw new Error('Id is required');
    }
    let url = `${this.baseUrl}/GetStudentResourceSupport?personId=${id}`;
    if (schoolYear){
      url += `&schoolYear=${schoolYear}`;
    }

    return this.$http.get(url);
  }

  chechForExistingByPerson(personId, schoolYear) {
    return this.$http.get(`${this.baseUrl}/ChechForExistingByPerson?personId=${personId}&schoolYear=${schoolYear}`);
  }
}

export default new ResourceSupportService();
