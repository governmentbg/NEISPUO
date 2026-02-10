import http from './http.service';

class AdminService {
    constructor() {
      this.$http = http;
      this.baseUrl = "/api/admin";
    }

    getServerInfo() {
      return this.$http.get(`${this.baseUrl}/GetServerInfo`);
    }

    getDirectorDashboard() {
        return this.$http.get(`${this.baseUrl}/GetDirectorDashboard`);
    }

    dischargeStudents(model){
        if (!model) {
            throw new Error('Model is required');
         }

        return this.$http.post(`${this.baseUrl}/DischargeStudents`, model);
    }

    admissionStudents(model){
        if (!model) {
            throw new Error('Model is required');
         }

        return this.$http.post(`${this.baseUrl}/AdmissionStudents`, model);
    }

    getDBDgml(){
      return this.$http.get("/api/administration/DBDgml");
    }

    getInstitutionSopEnrollmentsCount() {
      return this.$http.get(`${this.baseUrl}/GetInstitutiontSopEnrollmentsCount`);
    }

    getClassGroupStats() {
      return this.$http.get(`${this.baseUrl}/GetClassGroupStats`);
    }

    getDiplomaStats() {
      return this.$http.get(`${this.baseUrl}/GetDiplomaStats`);
    }

    getStudentsForDischarge() {
      return this.$http.get(`${this.baseUrl}/GetStudentsForDischarge`);
    }

    getAllActiveCampaigns() {
      return this.$http.get(`${this.baseUrl}/GetAllActiveCampaigns`);
    }

    getStudentsCount() {
      return this.$http.get(`${this.baseUrl}/GetStudentsCount`);
    }

    getStudentsCountGroupByClassType() {
      return this.$http.get(`${this.baseUrl}/GetStudentsCountGroupByClassType`);
    }

    getStudentsCountByClassType(classTypeId) {
      return this.$http.get(`${this.baseUrl}/GetStudentsCountByClassType?classTypeId=${classTypeId || ''}`);
    }
}

export default new AdminService();
