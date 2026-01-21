import http from './http.service';

class InstitutionService {
    constructor() {
      this.$http = http;
      this.baseUrl = "/api/institution";
    }

    getById(id) {
        if (!id) {
            throw new Error('Id is required');
        }

        return this.$http.get(`${this.baseUrl}/GetById/?institutionId=${id}`);
    }

    getCurrentYear(id) {
      if(!id || isNaN(Number(id))) {
        return this.$http.get(`${this.baseUrl}/GetCurrentYear`);
      } else {
        return this.$http.get(`${this.baseUrl}/GetCurrentYear?institutionId=${id}`);
      }
    }

    getDropdownModelById(id) {
        if (!id) {
            throw new Error('Id is required');
        }

        return this.$http.get(`${this.baseUrl}/GetDropdownModelById/?institutionId=${id}`);
    }

    autoNumber(classId){
        if (!classId) {
            throw new Error('classId is required');
        }

        return this.$http.get(`${this.baseUrl}/AutoNumber/?classId=${classId}`);
    }

    getClassGroups(institutionId, schoolYear){
        if (!institutionId) {
            throw new Error('institutionid is required');
        }

        return this.$http.get(`${this.baseUrl}/GetClassGroups?institutionId=${institutionId}&schoolYear=${schoolYear}`);
    }

    getClassGroupsForAdditionalEnrollment(params){
        if (!params.personId) {
            throw new Error('PersonId is required');
        }

        const queryString = `personId=${params.personId}&schoolYear=${params.schoolYear}`;

        return this.$http.get(`${this.baseUrl}/GetClassGroupsForAdditionalEnrollment?${queryString}`);
    }

    getInstitutions(){
        return this.$http.get(`${this.baseUrl}/GetInstitutions`);
    }

    getFullDetails(id){
        if (!id) {
            throw new Error('id is required');
        }

        return this.$http.get(`${this.baseUrl}/GetFullDetails?id=${id}`);
    }

    getCurrentForStudent(studentId) {
        if (!studentId) {
            throw new Error('studentId is required');
        }

        return this.$http.get(`${this.baseUrl}/GetCurrentForStudent?studentId=${studentId}`);
    }

    getLoggedUserInstitution(){
        return this.$http.get(`${this.baseUrl}/GetLoggedUserInstitution`);
    }

    hasExternalSoProviderForLoggedInstitution(){
      return this.$http.get(`${this.baseUrl}/HasExternalSoProviderForLoggedInstitution`);
  }
}

export default new InstitutionService();
