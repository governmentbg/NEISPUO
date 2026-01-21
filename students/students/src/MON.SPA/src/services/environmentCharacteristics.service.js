import http from './http.service';

class EnvironmentCharacteristics {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/environmentCharacteristics";
  }

  update(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.put(`${this.baseUrl}/UpdateEnvironmentCharacteristics`, model);
  }

  getStudentEnvironmentCharacteristics(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/GetStudentEnvironmentCharacteristics?personId=${id}`);
  }

  getRelatives(personId) {
    if (!personId) {
      throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/GetRelatives?personId=${personId}`);
  }

  deleteRelative(relativeId,personId) {
    if (!relativeId || !personId) {
      throw new Error('RelativeId is required');
    }

    return this.$http.delete(`${this.baseUrl}/DeleteRelative?relativeId=${relativeId}&personId=${personId}`);
  }

  addRelative(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.post(`${this.baseUrl}/AddRelative`, model);
  }

  updateRelative(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.put(`${this.baseUrl}/UpdateRelative`, model);
  }

  getRelative(relativeId,personId) {
    if (!relativeId || !personId) {
      throw new Error('RelativeId is required');
    }

    return this.$http.get(`${this.baseUrl}/GetRelative?relativeId=${relativeId}&personId=${personId}`);
  }

  getRelatedRelativeByPin(pin, personId) {
    if (!pin) {
      throw new Error('Pin is required');
    }

    if (!personId) {
        throw new Error('PersonId is required');
    }

    return this.$http.get(`${this.baseUrl}/GetRelatedRelativeByPin?pin=${pin}&personId=${personId}`);
  }

  
}
  
export default new EnvironmentCharacteristics();
