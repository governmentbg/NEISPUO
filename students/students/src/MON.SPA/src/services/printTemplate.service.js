import http from './http.service';

class PrintTemplateService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/printTemplate";
  }

  getDefaultMargins(basicDocumentId,  reportForm){
    return this.$http.get(`${this.baseUrl}/getDefaultMargins?basicDocumentId=${basicDocumentId}&reportForm=${reportForm}`);
  }

  setDefaultMargins(margins){
    return this.$http.put(`${this.baseUrl}/setDefaultMargins`, margins);
  }

  getPrintTemplate(id) {
    let url = `${this.baseUrl}/getPrintTemplate?id=${id}`;

    return this.$http.get(url);
  }

  list(){
    let url = `${this.baseUrl}/list`;

    return this.$http.get(url);
  }

  deletePrintTemplate(id) {
    return this.$http.delete(`${this.baseUrl}/Delete?id=${id}`);
  }

  create(model) {
    return this.$http.post(`${this.baseUrl}/Create`, model);
  }

  update(model){
    return this.$http.put( `${this.baseUrl}/Update`, model);
  }
}

export default new PrintTemplateService();
