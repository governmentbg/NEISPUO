import http from './http.service';

class AppConfigurationService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/AppConfiguration";
  }

  getValueByKey(key) {
    return this.$http.get(`${this.baseUrl}/getValueByKey?key=${key}`);
  }
}

export default new AppConfigurationService();