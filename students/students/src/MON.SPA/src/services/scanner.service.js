import http from './http.service';

class ScannerService {
  constructor() {
    this.$http = http;
    this.baseUrl = "http://127.0.0.1:5339/api/scanner";
  }

scan(outputType){

    if (!outputType) {
        throw new Error('outputType is required');
    }

    const model = {
        output: outputType
    };

    return this.$http.post(`${this.baseUrl}/scan`,  model);
  }
}

export default new ScannerService();