import http from './http.service';

class RelocationDocumentService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/relocationDocument";
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

  create(model) {
    if (!model) {
      throw new Error('Student relocation document model is required');
    }

    return this.$http.post(`${this.baseUrl}/Create`, model);
  }

  update(model) {
    if (!model) {
      throw new Error('Student relocation document model is required');
    }

    return this.$http.put(`${this.baseUrl}/Update`, model);
  }

  delete(id) {
    return this.$http.delete(`${this.baseUrl}/Delete?documentId=${id}`);
  }

  confirm(documentId) {
    if (!documentId) {
      throw new Error('Student relocation document Id is required');
    }

    return this.$http.put(`${this.baseUrl}/Confirm?id=${documentId}`);
  }

  getRelocationDocumentOptions(personId) {
    if (!personId) {
      throw new Error('PersonId is required');
    }

    return this.$http.get(`${this.baseUrl}/GetRelocationDocumentOptionsByPerson?personId=${personId}`);
  }

  getStudentCurrentTermGrades(relocationDocumentId) {
    return this.$http.get(`${this.baseUrl}/GetStudentCurrentTermGrades?relocationDocumentId=${relocationDocumentId}`);
  }

  getAbsences(documentId) {
    if (!documentId) {
      throw new Error('Student relocation document Id is required');
    }
    return this.$http.get(`${this.baseUrl}/GetAbsences?relocationDocumentId=${documentId}`);
  }

  getLodAssessmentsList(documentId) {
    if (!documentId) {
      throw new Error('Student relocation document Id is required');
    }
    return this.$http.get(`${this.baseUrl}/GetLodAssessmentsList?relocationDocumentId=${documentId}`);
  }
}

export default new RelocationDocumentService();
