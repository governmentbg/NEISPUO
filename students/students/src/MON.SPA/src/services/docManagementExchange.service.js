import http from './http.service';

class DocManagementExchangeService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/docManagementExchange";
  }

  getFreeForExchange(institutionId){
    if (!institutionId) {
      throw new Error('InstitutionId is required');
    }

    return this.$http.get(`${this.baseUrl}/FreeForExchange?institutionId=${institutionId}`);
  }

  createRequest(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.post(`${this.baseUrl}/CreateRequest`, model);
  }

  deleteRequest(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.delete(`${this.baseUrl}/DeleteRequest?id=${id}`);
  }

  approveRequest(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.post(`${this.baseUrl}/ApproveRequest`, model);
  }

  rejectRequest(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.post(`${this.baseUrl}/RejectRequest`, model);
  }

  generateProtocol(inputModel) {
    return this.$http.post(`${this.baseUrl}/generateProtocol`, inputModel, { responseType: 'blob' });
  }
}

export default new DocManagementExchangeService();
