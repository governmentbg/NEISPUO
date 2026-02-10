import http from './http.service';

class ErrorService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/error";
  }

  add(model) {
    if (!model) {
      throw new Error('ErrorModel model is required');
    }

    return this.$http.post(`${this.baseUrl}/Add`, model);
  }
}

export default new ErrorService;
