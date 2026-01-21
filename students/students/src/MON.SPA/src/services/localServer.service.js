import http from './http.service';

class LocalServerService {
  constructor() {
    this.$http = http;
    this.baseUrl = "http://127.0.0.1:5339/api";
  }

  version() {
    return this.$http.get(`${this.baseUrl}/server/version`, {timeout: 2000});
  }

  settings() {
    return this.$http.get(`${this.baseUrl}/server/settings`, {timeout: 2000});
  }

  edition() {
    return this.$http.get(`${this.baseUrl}/server/edition`, {timeout: 2000});
  }

  capabilities() {
    return this.$http.get(`${this.baseUrl}/server/caps`, {timeout: 2000});
  }
}

export default new LocalServerService();
