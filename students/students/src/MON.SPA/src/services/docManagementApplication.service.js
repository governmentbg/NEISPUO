import http from './http.service';

class DocManagementApplicationService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/docManagementApplication";
  }

 getById(id){
    if (!id) {
        throw new Error('sanctionId is required');
    }

    return this.$http.get(`${this.baseUrl}/GetById?id=${id}`);
  }

  getBasicDocuments() {
    return this.$http.get(`${this.baseUrl}/GetBasicDocuments`);
  }

  create(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.post(`${this.baseUrl}/Create`, model);
  }

  update(model){
    if (!model) {
        throw new Error('Model is required');
    }

    return this.$http.put( `${this.baseUrl}/Update`, model);
  }

  attachApplicationReport(model){
    if (!model) {
        throw new Error('Model is required');
    }

    return this.$http.put( `${this.baseUrl}/attachApplicationReport`, model);
  }

  reportDelivery(model){
    if (!model) {
        throw new Error('Model is required');
    }

    return this.$http.put( `${this.baseUrl}/ReportDelivery`, model);
  }

  delete(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.delete(`${this.baseUrl}/Delete?id=${id}`);
  }

  generateApplicationReport(inputModel) {
    return this.$http.post(`${this.baseUrl}/GenerateApplicationReport`, inputModel, { responseType: 'blob' });
  }

  generateReport(inputModel) {
    // Приложение № 6 към чл. 52, ал. 1
    return this.$http.post(`${this.baseUrl}/GenerateReport`, inputModel, { responseType: 'blob' });
  }

  generateDestructionProtocol(inputModel) {
    // Протоколът за унищожаване е формуляр
    return this.$http.post(`${this.baseUrl}/GenerateDestructionProtocol`, inputModel, { responseType: 'blob' });
  }

  statuses(id){
    return this.$http.get(`${this.baseUrl}/Statuses?id=${id}`);
  }

  returnForCorrection(model){
    if (!model) {
        throw new Error('Model is required');
    }

    return this.$http.put( `${this.baseUrl}/ReturnForCorection`, model);
  }

  actionResponse(model){
    if (!model) {
        throw new Error('Model is required');
    }

    return this.$http.put(`${this.baseUrl}/ActionResponse`, model);
  }

  submit(model){
      return this.$http.put(`${this.baseUrl}/Submit`, model);
  }

  approve(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.post(`${this.baseUrl}/Approve`, model);
  }

  reject(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.post(`${this.baseUrl}/Reject`, model);
  }
}

export default new DocManagementApplicationService();
