import http from './http.service';

class AspService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/asp";
  }

  readDateAndMonthFromBenefitsFile(formData) {
    if (!formData) {
      throw new Error('You must choose a file!');
    }

    return this.$http.post(`${this.baseUrl}/ReadBenefitsFile`, formData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
  }

  importBenefits(formData) {
    if (!formData) {
      throw new Error('You must choose a file!');
    }

    return this.$http.post(`${this.baseUrl}/ImportBenefits`, formData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
  }

  loadAspConfirmations(sessionInfoModel) {
    if (!sessionInfoModel) {
      throw new Error('Session info model is required!');
    }

    return this.$http.post(`${this.baseUrl}/loadAspConfirmations`, sessionInfoModel);
  }

  getImportedBenefitsFiles() {
    return this.$http.get(`${this.baseUrl}/GetImportedBenefitsFiles/`);
  }

  getImportedBenefitsFileMetaData(importedFileId) {
    return this.$http.get(`${this.baseUrl}/GetImportedBenefitsFileMetaData/?importedFileId=${importedFileId}`);
  }

  getImportedBenefitsDetails(query) {
    return this.$http.get(`${this.baseUrl}/GetImportedBenefitsDetails${query}`, undefined, false);
  }

  updateBenefit(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.put(`${this.baseUrl}/UpdateBenefit/`, model);
  }

  updateImportedBenefitsFileMetaData(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.put(`${this.baseUrl}/UpdateImportedBenefitsFileMetaData/`, model);
  }

  exportApprovedMonthlyBenefits(campaignId) {
    if (!campaignId) {
      throw new Error('Campaign ID is required');
    }
    return this.$http.put(`${this.baseUrl}/exportApprovedMonthlyBenefits?campaignId=${campaignId}`);
  }

  exportEnrolledStudents(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.post(`${this.baseUrl}/ExportEnrolledStudents`, model);
  }

  checkForExistingEnrolledStudentsExport(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.post(`${this.baseUrl}/CheckForExistingEnrolledStudentsExport`, model);
  }

  getEnrolledStudentsExportFiles(schoolYear) {
    const relativeUrl = schoolYear ? `GetEnrolledStudentsExportFiles?schoolYear=${schoolYear}` : 'GetEnrolledStudentsExportFiles';
    return this.$http.get(`${this.baseUrl}/${relativeUrl}`);
  }

  constructAspBenefitsConfirmationAsXml(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.post(`${this.baseUrl}/ConstructAspBenefitsAsXml`, model);
  }

  setAspBenefitsSigningAtrributes(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.put(`${this.baseUrl}/SetAspBenefitsSigningAtrributes`, model);
  }

  removeAspBenefitsSigningAtrributes(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.post(`${this.baseUrl}/RemoveAspBenefitsSigningAtrributes`, model);
  }

  deleteImporedFileRecord(fileId) {
    if (!fileId) {
      throw new Error('FileId is required');
    }

    return this.$http.delete(`${this.baseUrl}/DeleteImporedFileRecord?fileId=${fileId}`);
  }

  uploadSubmittedData(formData) {
    if (!formData) {
      throw new Error('You must choose a file!');
    }

    return this.$http.post(`${this.baseUrl}/uploadSubmittedData`, formData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
  }

  getCampaignStats(id) {
    if (!id) {
      throw new Error('Id is required');
    }
    return this.$http.get(`${this.baseUrl}/getCampaignStats?id=${id}`);
  }

  getMonSession(schoolYear, month, infoType) {
    if (!schoolYear) {
      throw new Error('Schoolyear is required');
    }

    if (!month) {
      throw new Error('month is required');
    }

    return this.$http.get(`${this.baseUrl}/GetMonSession?schoolYear=${schoolYear}&month=${month}&infoType=${infoType || ''}`);
  }
}

export default new AspService();
