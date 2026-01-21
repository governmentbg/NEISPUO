import http from './http.service';

class StudentClassService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/studentClass";
  }

  getById(id){
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/GetById?id=${id}`);
  }

  getHistoryById(id){
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/GetHistoryById?id=${id}`);
  }

  getByPersonId(searchModel) {
    if (!searchModel) {
      throw new Error('Search model is required');
    }

    return this.$http.post(`${this.baseUrl}/GetByPersonId`, searchModel);
  }

  getMainForPersonAndLoggedInstitution(personId, schoolYear) {
    if (!personId) {
      throw new Error('PersonId is required');
    }

    if (!schoolYear) {
      throw new Error('School year is required');
    }

    return this.$http.get(`${this.baseUrl}/GetMainForPersonAndLoggedInstitution?personId=${personId}&schoolYear=${schoolYear}`);
  }

  update(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.put(`${this.baseUrl}/Update`, model);
  }

  enrollInClass(studentClass){
    if (!studentClass) {
      throw new Error('StudentClass is required');
    }

    return this.$http.post(`${this.baseUrl}/EnrollInClass`, studentClass);
  }

  enrollInCplrClass(studentClass) {
    if (!studentClass) {
      throw new Error('StudentClass is required');
    }

    return this.$http.post(`${this.baseUrl}/EnrollInCplrClass`, studentClass);
  }

  enrollInAdditionalClass(studentClass) {
    if (!studentClass) {
      throw new Error('StudentClass is required');
    }

    return this.$http.post(`${this.baseUrl}/EnrollInAdditionalClass`, studentClass);
  }

  еnrollInCplrAdditionalClass(studentClass) {
    if (!studentClass) {
      throw new Error('StudentClass is required');
    }

    return this.$http.post(`${this.baseUrl}/EnrollInCplrAdditionalClass`, studentClass);
  }

  updateAdditionalClass(studentClass) {
    if (!studentClass) {
      throw new Error('StudentClass is required');
    }

    return this.$http.put(`${this.baseUrl}/UpdateAdditionalClass`, studentClass);
  }

  changeAdditionalClass(studentClass) {
    if (!studentClass) {
      throw new Error('StudentClass is required');
    }

    return this.$http.post(`${this.baseUrl}/ChangeAdditionalClass`, studentClass);
  }

  getCurrentClassSummaryById(studentClassId) {
    if(!studentClassId) {
      throw new Error('studentClassId is required');
    }

    return this.$http.get(`${this.baseUrl}/GetCurrentClassSummaryById?studentClassId=${studentClassId}`);
  }


  getPersonBasicClasses(personId, forCurrentInstitution) {
    if(!personId) throw new Error('PersonId is required!');
    return this.$http.get(`${this.baseUrl}/GetPersonBasicClasses?personId=${personId}&forCurrentInstitution=${forCurrentInstitution}`);
  }


  changeClassInInstitution(studentClass){
    if (!studentClass) {
      throw new Error('StudentClass is required');
    }
    return this.$http.post(`${this.baseUrl}/ChangeClassInInstitution`, studentClass);
  }

  deleteHistoryRecord(id){
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.delete(`${this.baseUrl}/DeleteHistoryRecord?id=${id}`);
  }

  addToNewClassBtnVisibilityCheck(personId) {
    if (!personId) {
      throw new Error('PersonId is required');
    }

    return this.$http.get(`${this.baseUrl}/AddToNewClassBtnVisibilityCheck?personId=${personId}`);
  }

  unenrollFromClass(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.put(`${this.baseUrl}/UnenrollFromClass`, model);
  }

  deleteAdditionalClass(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.delete(`${this.baseUrl}/Delete?id=${id}`);
  }

  unenrollSelected(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.put(`${this.baseUrl}/UnenrollSelected`, model);
  }

  enrollSelected(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.post(`${this.baseUrl}/EnrollSelected`, model);
  }

  getDualFormCompanies(studentClassId){
    if (!studentClassId) {
      throw new Error('StudentClassId is required');
    }

    return this.$http.get(`${this.baseUrl}/GetDualFormCompanies?studentClassId=${studentClassId}`);
  }

  changePosition(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.put(`${this.baseUrl}/ChangePosition`, model);
  }
}

export default new StudentClassService();
