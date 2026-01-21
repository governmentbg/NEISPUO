import http from './http.service';

class FileService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/file";
  }

  getById(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/GetById?id=${id}`);
  }

  upload(file) {
    if (!file) {
      throw new Error('File is required');
    }

    const formData = new FormData();
    formData.append('file', file);
    const headers = { 'Content-Type': 'multipart/form-data' };
    return this.$http.post(`${this.baseUrl}/upload`, formData, { headers });
  }

}

export default new FileService();
