import QueryString from "query-string";
import http from './http.service';

class StudentService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/student";
  }

  getById(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/GetById/?id=${id}`);
  }

  getSummaryById(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/GetSummaryById/?id=${id}`);
  }

  getAll() {
    return this.$http.get(`${this.baseUrl}/GetAll`);
  }

  create(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.post(`${this.baseUrl}/Post`, model);
  }

  updateBasicDetails(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.put(`${this.baseUrl}/UpdateBasicDetails`, model);
  }

  updateInternationalProtection(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.put(`${this.baseUrl}/UpdateInternationalProtection`, model);
  }

  isPinValid(pinTypeId, pin) {
    if(!pin) {
      throw new Error('Pin is required');
    }

    if(!Number.isInteger(pinTypeId)) {
      throw new Error('PinType is required');
    }

    return this.$http.get(`${this.baseUrl}/IsPinValid?pinTypeId=${pinTypeId}&pin=${pin}`);
  }

  checkPinUniqueness(pin) {
    if(!pin) {
      throw new Error('Pin is required');
    }

    return this.$http.get(`${this.baseUrl}/CheckPinUniqueness?pin=${pin}`);
  }

  getBySearch(searchModel) {
    if (!searchModel) {
      throw new Error('Search model is required');
    }
    const url = `${this.baseUrl}/GetBySearch`;
    return this.$http.get(`${url}?${QueryString.stringify(searchModel)}`);
    // return this.$http.get(encodeURI(`${this.baseUrl}/GetBySearch` +
    //   `?pin=${searchModel.pin}&firstName=${searchModel.firstName}&middleName=${searchModel.middleName}&lastName=${searchModel.lastName}&exactMatch=${searchModel.exactMatch}`));
  }

  deleteById(id) {
    return this.$http.delete(`${this.baseUrl}/DeleteById?studentId=${id}`);
  }

  deleteAzureAccount(id) {
    return this.$http.delete(`${this.baseUrl}/DeleteAzureAccount?studentId=${id}`);
  }

  getStudentResourceSupportHistory(personId) {
    if (!personId) {
      throw new Error('Student personId is required');
    }

    return this.$http.get(`${this.baseUrl}/GetStudentResourceSupportHistory?personId=${personId}`);
  }

  getStudentSopHistory(personId) {
    if (!personId) {
      throw new Error('Student personId is required');
    }

    return this.$http.get(`${this.baseUrl}/GetStudentSopHistory?personId=${personId}`);
  }

  getPersonDataById(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/GetPersonDataById/?id=${id}`);
  }

  getStudentEducationByPersonId(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/GetStudentEducation?personId=${id}`);
  }

  getStudentInternationalProtection(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/GetStudentInternationalProtection?personId=${id}`);
  }

  getStudentPersonalDataHistory(personId) {
    if (!personId) {
      throw new Error('Student personId is required');
    }

    return this.$http.get(`${this.baseUrl}/GetStudentPersonalDataHistory?personId=${personId}`);
  }

  isStudentInCurrentInstitution(personId) {
    if (!personId) {
      throw new Error('Student personId is required');
    }

    return this.$http.get(`${this.baseUrl}/IsStudentInCurrentInstitution?personId=${personId}`);
  }

  canEditStudentPersonalDetails(personId) {
    if (!personId) {
      throw new Error('Student personId is required');
    }

    return this.$http.get(`${this.baseUrl}/CanEditStudentPersonalDetails?personId=${personId}`);
  }

  initialEnrollmentSecretBtnVisibilityCheck(personId) {
    if (!personId) {
      throw new Error('Student personId is required');
    }

    return this.$http.get(`${this.baseUrl}/InitialEnrollmentSecretBtnVisibilityCheck?personId=${personId}`);
  }
}

export default new StudentService();
