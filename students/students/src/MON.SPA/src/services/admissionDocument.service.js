import http from './http.service';

class AdmissionDocumentService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/admissionDocument";
  }

  getById(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/GetById?id=${id}`);
  }

  getByPersonId(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/GetByPersonId?personId=${id}`);
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

  create(model) {
    if (!model) {
      throw new Error('Student admission document model is required');
    }

    return this.$http.post(`${this.baseUrl}/Create`, model);
  }

  update(model) {
    if (!model) {
      throw new Error('Student admission document model is required');
    }

    return this.$http.put(`${this.baseUrl}/Update`, model);
  }

  delete(id) {
    return this.$http.delete(`${this.baseUrl}/Delete?documentId=${id}`);
  }

  confirm(model) {
    if (!model) {
      throw new Error('Student admission document model is required');
    }

    return this.$http.put(`${this.baseUrl}/Confirm?id=${model.id}`);
  }

  getListForRelocationDocument(relocationDocumentId) {
    if (!relocationDocumentId) {
      throw new Error('relocationDocumentId is required');
    }

    return this.$http.get(`${this.baseUrl}/getListForRelocationDocument?relocationDocumentId=${relocationDocumentId}`);
  }
}

export default new AdmissionDocumentService();