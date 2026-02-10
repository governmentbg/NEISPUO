import http from './http.service';
import QueryString from "query-string";

class HealthInsuranceService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/healthInsurance";
  }

  list(schoolYear, month) {
    const userListInput = {
        year: schoolYear,
        month: month
    };

    return this.$http.get(`${this.baseUrl}/List?${QueryString.stringify(userListInput)}`);
  }

  generateFile(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.post(`${this.baseUrl}/GenerateHealthInsuranceFile`, model);
  }


  delete(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.delete(`${this.baseUrl}/Delete?id=${id}`);
  }
}

export default new HealthInsuranceService();
