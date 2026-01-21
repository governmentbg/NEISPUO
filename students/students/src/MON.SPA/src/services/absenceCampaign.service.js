import http from './http.service';

class AbsenceCampaignService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/absenceCampaign";
  }

  getById(id){
    if (!id) {
        throw new Error('sanctionId is required');
    }

    return this.$http.get(`${this.baseUrl}/GetById?id=${id}`);
  }

  getDetailsById(id) {
    if (!id) {
      throw new Error('sanctionId is required');
    }

    return this.$http.get(`${this.baseUrl}/GetDetailsById?id=${id}`);
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

  getStats(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/getStats?id=${id}`);
  }

  getAspSession(schoolYear, month, infoType) {
    if (!schoolYear) {
      throw new Error('Schoolyear is required');
    }

    if (!month) {
      throw new Error('month is required');
    }

    return this.$http.get(`${this.baseUrl}/GetAspSession?schoolYear=${schoolYear}&month=${month}&infoType=${infoType || ''}`);
  }
}

export default new AbsenceCampaignService();
