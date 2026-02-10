import http from './http.service';
import QueryString from 'query-string';

class DiplomaService {

  constructor() {
    this.$http = http;
    this.baseUrl = '/api/diploma';
  }

  constructDiplomaByIdAsXml(diplomaId) {
    if (!diplomaId) {
      throw new Error('DiplomaId is required');
    }

    return this.$http.get(`${this.baseUrl}/ConstructDiplomaByIdAsXml/?id=${diplomaId}`);
  }

  create(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.post(`${this.baseUrl}/Create`, model);
  }

  update(model) {

    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.put(`${this.baseUrl}/Update`, model);
  }

  delete(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.delete(`${this.baseUrl}/Delete/?id=${id}`);
  }

  getCreateModel(personId, templateId, basicDocumentId, basicClassId) {
    const params = {};
    if (personId) params.personId = personId;
    if (templateId) params.templateId = templateId;
    if (basicDocumentId) params.basicDocumentId = basicDocumentId;
    if (basicClassId) params.basicClassId = basicClassId;

    return this.$http.get(`${this.baseUrl}/GetCreateModel?${QueryString.stringify(params)}`);
  }

  getUpdateModel(diplomaId) {
    if (!diplomaId) {
      throw new Error('DiplomaId is required');
    }

    return this.$http.get(`${this.baseUrl}/GetUpdateModel/?diplomaId=${diplomaId}`);
  }

  getOriginalDocuments(personId, personalId, personalIdType, mainBasicDocuments) {
    const params = {
      mainBasicDocuments: mainBasicDocuments,
    };
    if (personId) params.personId = personId;
    if (personalId) params.personalId = personalId;
    if (personalIdType) params.personalIdType = personalIdType;

    return this.$http.get(`${this.baseUrl}/GetOriginalDocuments?${QueryString.stringify(params)}`);
  }

  // Delete
  getDiplomaDocuments(diplomaId){
    if (!diplomaId) {
      throw new Error('diplomaId is required');
    }

    return this.$http.get(`${this.baseUrl}/GetDiplomaDocuments/?diplomaId=${diplomaId}`);
  }

  // Delete
  uploadDiplomaDocument(payload){
    return this.$http.post(`${this.baseUrl}/uploadDiplomaDocument`, payload, {
      headers: {
        "Content-Type": "multipart/form-data",
      }
    });
  }

  uploadDiplomaImage(file, diplomaId, description){
    return this.$http.post(`${this.baseUrl}/uploadDiplomaImage`, {document: file, diplomaId: diplomaId, description: description});
  }

  reorderDiplomaDocuments(diplomaId, orderedDocumentsArray){
    return this.$http.post(`${this.baseUrl}/ReorderDiplomaDocuments`, {id: diplomaId, documentPositions: orderedDocumentsArray});
  }

  removeDiplomaDocument(id){
    return this.$http.get(`${this.baseUrl}/RemoveDiplomaDocument/?id=${id}`);
  }

  getBasicDetails(diplomaId) {
    if (!diplomaId) {
      throw new Error('DiplomaId is required');
    }

    return this.$http.get(`${this.baseUrl}/getBasicDetails/?diplomaId=${diplomaId}`);
  }

  updateDiplomaFinalizationSteps(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.put(`${this.baseUrl}/UpdateDiplomaFinalizationSteps`, model);
  }

  getDiplomaSigningData(id){
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/GetDiplomaSigningData/?id=${id}`);
  }

  getDiplomaFinalizationDetailsById(diplomaId) {
    if (!diplomaId) {
      throw new Error('diplomaId is required');
    }

    return this.$http.get(`${this.baseUrl}/GetDiplomaFinalizationDetailsById/?diplomaId=${diplomaId}`);
  }

  anullDiploma(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.put(`${this.baseUrl}/AnullDiploma`, model);
  }

  generateApplicationFile(diplomaId) {
    return this.$http.get(`${this.baseUrl}/GenerateApplicationFile?diplomaId=${diplomaId}`, { responseType: 'blob' });
  }

  generateRegBookExport(schoolYear, basicDocumentId, regBookType) {
    return this.$http.get(`${this.baseUrl}/GenerateRegBookExport?year=${schoolYear}&basicDocumentId=${basicDocumentId}&regBookType=${regBookType}`, { responseType: 'blob' });
  }

  import(formData) {
    if (!formData) {
      throw new Error('You must choose a file!');
    }

    return this.$http.post(`${this.baseUrl}/import`, formData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
  }

  setAsEditable(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.put(`${this.baseUrl}/SetAsEditable`, model);
  }

  getAdditionalDocumentDetails(personId, basicDocumentId) {
    if(!personId) {
      throw new Error('PersonId is required');
    }

    if(!basicDocumentId) {
      throw new Error('BasicDocumentId is required');
    }

    return this.$http.get(`${this.baseUrl}/GetAdditionalDocumentDetails?personId=${personId}&basicDocumentId=${basicDocumentId}`);
  }

  regBookList(regBookType) {
    if(!regBookType) {
      throw new Error('TypeOfRegBook is required');
    }

    return this.$http.get(`${this.baseUrl}/GetRegBookBasicDetails?typeOfRegBook=${regBookType}`);
  }
}

export default new DiplomaService();
