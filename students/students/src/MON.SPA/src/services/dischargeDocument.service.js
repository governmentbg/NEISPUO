import http from './http.service';

class DischargeDocumentService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/dischargeDocument";
  }

  getById(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/GetById?id=${id}`);
  }

  getListForPerson(personId) {
    if (!personId) {
      throw new Error('PersonId is required');
    }

    return this.$http.get(`${this.baseUrl}/GetListForPerson/?personId=${personId}`);
  }
  
  create(model) {
    if (!model) {
      throw new Error('Student discharge document model is required');
    }

    return this.$http.post(`${this.baseUrl}/Create`, model);
  }

  update(model) {
    if (!model) {
      throw new Error('Student discharge document model is required');
    }

    return this.$http.put(`${this.baseUrl}/Update`, model);
  }

  delete(id) {
    return this.$http.delete(`${this.baseUrl}/Delete?id=${id}`);
  }

  confirm(documentId) {
    if (!documentId) {
      throw new Error('Student discharge document Id is required');
    }

    return this.$http.put(`${this.baseUrl}/Confirm?id=${documentId}`);
  }
}

export default new DischargeDocumentService();