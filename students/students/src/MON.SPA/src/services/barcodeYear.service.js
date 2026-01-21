import http from './http.service';

class BarcodeYearService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/BarcodeYear";
  }

  getBarcodeYears(basicDocumentId) {
    if (!basicDocumentId) {
      throw new Error('basicDocumentId is required');
    }

    let url = `${this.baseUrl}/GetBarcodeYears?basicDocumentId=${basicDocumentId}`;
  
    return this.$http.get(url);
  }

  deleteBarcodeYear(id) {
    return this.$http.delete(`${this.baseUrl}/DeleteBarcodeYear?id=${id}`);
  }

  addBarcodeYear(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.post(`${this.baseUrl}/AddBarcodeYear`, model);
  }

  getBarcodeYear(barcodeYearId){
    if (!barcodeYearId) {
        throw new Error('barcodeYearId is required');
    }

    let url = `${this.baseUrl}/GetBarcodeYear?barcodeYearId=${barcodeYearId}`;
  
    return this.$http.get(url);
  }

  updateBarcodeYear(model){
    if (!model) {
        throw new Error('Model is required');
    }

    return this.$http.put( `${this.baseUrl}/UpdateBarcodeYear`, model);
  }
}

export default new BarcodeYearService();