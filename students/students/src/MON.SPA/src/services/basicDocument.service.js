import http from './http.service';

class BasicDocumentService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/basicDocument";
  }

  loadTemplate(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/loadTemplate?id=${id}`);
  }

  saveTemplate(model) {
    if (!model) {
      throw new Error('Id is required');
    }

    return this.$http.post(`${this.baseUrl}/saveTemplate`, model);
  }

  loadSchema(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/GetSchema?id=${id}`);
  }

  loadSchemaByTemplateId(templateId) {
    if (!templateId) {
      throw new Error('TemplateId is required');
    }

    return this.$http.get(`${this.baseUrl}/GetSchemaByTemplateId?templateId=${templateId}`);
  }

  includeInRegister(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.put(`${this.baseUrl}/IncludeInRegister?id=${id}`);
  }

  excludeFromRegister(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.put(`${this.baseUrl}/ExcludeFromRegister?id=${id}`);
  }

  getNextBasicDocumentSequence(basicDocumentId, count, regDate){
    if (!basicDocumentId){
      throw new Error('basicDocumentId is required');
    }

    let regDateStr = null;
    if(regDate){
      regDateStr = regDate.toISOString().split('T')[0];
    }

    return this.$http.get(`${this.baseUrl}/GetNextBasicDocumentSequence?basicDocumentId=${basicDocumentId}&count=${count}&regDate=${regDateStr}`);
  }

  deleteBasicDocumentSequence(id){
    if (!id){
      throw new Error('id is required');
    }

    return this.$http.delete(`${this.baseUrl}/DeleteBasicDocumentSequence?id=${id}`);
  }

  getById(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/GetById?id=${id}`);
  }
}

export default new BasicDocumentService();
