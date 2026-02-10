import http from './http.service';

class LodFinalizationService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/lodFinalization";
  }

  approveLod(model) {
    if(!model) {
      throw new Error('Model is required');
    }

    return this.$http.post(`${this.baseUrl}/approveLod`, model);
  }

  finalizeLod(model) {
    if(!model) {
      throw new Error('Model is required');
    }

    return this.$http.post(`${this.baseUrl}/finalizeLod`, model);
  }

  approveLodUndo(model) {
    if(!model) {
      throw new Error('Model is required');
    }

    return this.$http.post(`${this.baseUrl}/approveLodUndo`, model);
  }

  finalizeLodUndo(model) {
    if(!model) {
      throw new Error('Model is required');
    }

    return this.$http.post(`${this.baseUrl}/finalizeLodUndo`, model);
  }

  signLod(model) {
    if(!model) {
      throw new Error('Model is required');
    }

    return this.$http.post(`${this.baseUrl}/signLod`, model);
  }

  signLodUndo(model) {
    if(!model) {
      throw new Error('Model is required');
    }

    return this.$http.post(`${this.baseUrl}/signLodUndo`, model);
  }

  getByPersonId(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/getByPersonId?personId=${id}`);
  }
}

export default new LodFinalizationService();
