import http from './http.service';

class DocManagementAdditionalCampaignService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/docManagementAdditionalCampaign";
  }

  getById(id){
    if (!id) {
        throw new Error('sanctionId is required');
    }

    return this.$http.get(`${this.baseUrl}/GetById?id=${id}`);
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

  toggleManuallyActivation(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.put(`${this.baseUrl}/ToggleManuallyActivation`, model);
  }

  getActive() {
    return this.$http.get(`${this.baseUrl}/GetActive`);
  }

  delete(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.delete(`${this.baseUrl}/Delete?id=${id}`);
  }

  generateReport(inputModel) {
    return this.$http.post(`${this.baseUrl}/generateReport`, inputModel, { responseType: 'blob' });
  }
}

export default new DocManagementAdditionalCampaignService();
