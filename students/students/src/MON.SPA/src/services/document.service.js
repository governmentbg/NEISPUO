import http from './http.service';

class DocumentService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/document";
  }

  getById(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/GetById/?id=${id}`, { responseType: 'blob' });
  }

  addRelocationDocument(model) {
    if (!model) {
      throw new Error('Student relocation document model is required');
    }

    return this.$http.post(`${this.baseUrl}/AddRelocationDocument`, model);
  }

  updateRelocationDocument(model) {
    if (!model) {
      throw new Error('Student relocation document model is required');
    }

    return this.$http.put(`${this.baseUrl}/UpdateRelocationDocument`, model);
  }

  deleteRelocationDocumentById(id) {
    return this.$http.delete(`${this.baseUrl}/DeleteRelocationDocumentById?documentId=${id}`);
  }

  addDischargeDocument(model) {
    if (!model) {
      throw new Error('Student discharge document model is required');
    }

    return this.$http.post(`${this.baseUrl}/AddDischargeDocument`, model);
  }

  getRelocationDocuments(personId) {
    if (!personId) {
      throw new Error('PersonId is required');
    }

    return this.$http.get(`${this.baseUrl}/GetRelocationDocumentsByPersonId/?personId=${personId}`);
  }

  addResourceSupportDocument(model) {
    if (!model) {
      throw new Error('Student resource support document model is required');
    }

    return this.$http.post(`${this.baseUrl}/AddResourceSupportDocument`, model);
  }

  deleteResourceSupportDocumentById(id){
    return this.$http.delete(`${this.baseUrl}/DeleteResourceSupportDocumentById?documentId=${id}`);
  }

  getTestDocuments() {
    return this.$http.get(`${this.baseUrl}/testFileManager`);
  }

  postTestDocuments(files) {
    return this.$http.post(`${this.baseUrl}/testPostFileManager`, files);
  }

  checkForExistingAdmissionDocument(personId, institutionId) {
    if (!personId || !institutionId) {
      throw new Error('PersonId and InstitutionId are required');
    }

    return this.$http.get(`${this.baseUrl}/checkForExistingAdmissionDocument?personId=${personId}&institutionId=${institutionId}`);
  }

  checkForAdmissionDocumentInTheSameInstitution(personId, institutionId) {
    if (!personId || !institutionId) {
      throw new Error('PersonId and InstitutionId are required');
    }

    return this.$http.get(`${this.baseUrl}/checkForAdmissionDocumentInTheSameInstitution?personId=${personId}&institutionId=${institutionId}`);
  }
}
  
export default new DocumentService();