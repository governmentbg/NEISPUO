import http from './http.service';

class CertificateService {
  constructor() {
    this.$http = http;
    this.baseUrl = "http://127.0.0.1:5339/api/certificate";
  }

  signXml(xml){
    const model = {
        xml: xml
    };

    return this.$http.post(`${this.baseUrl}/sign`, model);
  }

  signXmlThumbprint(xml, thumbprint) {
    const model = {
        xml: xml,
        thumbprint: thumbprint
    };

    return this.$http.post(`${this.baseUrl}/sign`, model);
  }

  signDocx(docx, options) {
    // docx - base64 string
    const model = {
      contents: docx,
      options: options || {}
    };

    return this.$http.post(`${this.baseUrl}/signDocx`, model);
  }

  signDocxThumbprint(docx, thumbprint, options) {
    // docx - base64 string
    const model = {
      contents: docx,
      thumbprint: thumbprint,
      options: options || {}
    };

    return this.$http.post(`${this.baseUrl}/signDocx`, model);
  }

  create(model) {
    if (!model) {
        throw new Error('Model is required!');
    }

    return this.$http.post(`/api/certificate/Create`, model);
  }

  verify(xml){
    return this.$http.post(`/api/certificate/verify`, {xml: xml});
  }

  delete(id) {
    if (!id) {
        throw new Error('Id is required!');
    }

    return this.$http.delete(`/api/certificate/Delete?id=${id}`);
  }

  download(id){
    if (!id) {
      throw new Error('Id is required!');
    }
    return this.$http.get(`/api/certificate/Download?id=${id}`);
  }

  getById(id){
    if (!id) {
      throw new Error('Id is required!');
    }
    return this.$http.get(`/api/certificate/GetById?id=${id}`);
  }

  update(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.put(`/api/certificate/Update`, model);
  }


}



export default new CertificateService();
