import http from './http.service';

class AbsenceService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/absence";
  }

  getAbsencesForClass(classId, schoolYear, month) {
    return this.$http.get(`${this.baseUrl}/getAbsencesForClass/?classId=${classId}&schoolYear=${schoolYear}&month=${month}`);
  }

  getStudentAbsences(query) {
    return this.$http.get(`${this.baseUrl}/getStudentAbsences${query}`, undefined, false);
  }

  create(studentAbsenceModel) {
    return this.$http.post(
      `${this.baseUrl}/Create`,
      studentAbsenceModel
    );
  }

  update(studentAbsenceModel) {
    return this.$http.put(
      `${this.baseUrl}/Update`,
      studentAbsenceModel
    );
  }

  upload(formData) {
    if (!formData) {
      throw new Error('You must choose a file!');
    }

    return this.$http.post(`${this.baseUrl}/importAbsences`, formData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
  }

  readDateAndMonthFromFile(formData) {
    if (!formData) {
      throw new Error('You must choose a file!');
    }

    return this.$http.post(`${this.baseUrl}/readAbsenceFile`, formData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
  }

  importAbsencesFromSchoolBooks(schoolYear, month) {
    return this.$http.get(`${this.baseUrl}/importAbsencesFromSchoolBooks/?schoolYear=${schoolYear}&month=${month}`);
  }

  getImportDetails(absenceImportId) {
    return this.$http.get(`${this.baseUrl}/GetImportDetails?absenceImportId=${absenceImportId}`);
  }

  getStudentAbsencesHistory(absenceId) {
    return this.$http.get(`${this.baseUrl}/getStudentAbsencesHistory/?absenceId=${absenceId}`);
  }

  getExportedAbsencesFiles(itemsPerPage, page) {
    return this.$http.get(`${this.baseUrl}/GetExportedAbsencesFiles/?itemsPerPage=${itemsPerPage}&page=${page}`);
  }

  exportAbsencesToFile(schoolYear, month) {
    return this.$http.post(`${this.baseUrl}/ExportAbsencesToFile/?schoolYear=${schoolYear}&month=${month}`);
  }

  copyAspAsking(schoolYear, month) {
    return this.$http.post(`${this.baseUrl}/CopyAspAsking?schoolYear=${schoolYear}&month=${month}`);
  }

  deleteAbsenceImport(id) {
    return this.$http.delete(`${this.baseUrl}/DeleteAbsenceImport?id=${id}`);
  }

  getManualImportSampleData(schoolYear, month) {
    return this.$http.get(`${this.baseUrl}/GetManualImportSampleData?schoolYear=${schoolYear}&month=${month}`);
  }

  importAbsencesFromManualEntry(schoolYear, month) {
    return this.$http.get(`${this.baseUrl}/importAbsencesFromManualEntry/?schoolYear=${schoolYear}&month=${month}`);
  }

  constructAbsenceImportAsXml(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.post(`${this.baseUrl}/ConstructAbsenceImportAsXml`, model);
  }

  constructAbsenceExportAsXml(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.post(`${this.baseUrl}/ConstructAbsenceExportAsXml`, model);
  }

  constructNoAbsenceImportAsXml(absenceImportId) {
    if (!absenceImportId) {
      throw new Error('AbsenceImportId is required');
    }

    return this.$http.get(`${this.baseUrl}/ConstructNoAbsencesImportAsXml?absenceImportId=${absenceImportId}`);
  }

  setAbsenceImportSigningAtrributes(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.put(`${this.baseUrl}/SetAbsenceImportSigningAtrributes`, model);
  }

  setAbsenceExportSigningAtrributes(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.put(`${this.baseUrl}/SetAbsenceExportSigningAtrributes`, model);
  }

  getCampaign(campaignId) {
    if (!campaignId) {
      throw new Error('CampaignId is required');
    }

    return this.$http.put(`${this.baseUrl}/GetCampaignById`, campaignId);
  }

  createNoAbsencesImport(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.post(`${this.baseUrl}/CreateNoAbsencesImport`, model);
  }
}

export default new AbsenceService();
