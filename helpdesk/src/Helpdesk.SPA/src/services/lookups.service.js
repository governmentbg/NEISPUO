import http from './http.service';
class LookupsService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/lookups";
  }

  getStatuses() {
    return this.$http.get(`${this.baseUrl}/GetStatuses`);
  }

  getPriorities() {
    return this.$http.get(`${this.baseUrl}/GetPriorities`);
  }

  getCategories() {
    return this.$http.get(`${this.baseUrl}/GetCategories`);
  }

  getCategory(id) {
    return this.$http.get(`${this.baseUrl}/GetCategory?id=${id}`);
  }
}

export default new LookupsService();
