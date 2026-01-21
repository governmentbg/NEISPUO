namespace Domain.Models
{
    using DocumentFormat.OpenXml.Drawing;
    using System.Collections.Generic;

    public class LodFileModel
    {
        public LodFileModel()
        {
            schoolYearAssessments = new List<SchoolYearAssessmentsModel>();
            schoolYearExternalEvaluations = new List<SchoolYearExternalEvaluationsModel>();
            schoolYearSelfEduFormAsssessments = new List<SchoolYearAssessmentsModel>();
            schoolYearAbsences = new List<SchoolYearAbsencesModel>();
            schoolYearCommonPersonDevelopmentSupports = new List<SchoolYearCommonPersonDevelopmentSupportModel>();
            schoolYearAdditionalPersonDevelopmentSupports = new List<SchoolYearAdditionalPersonDevelopmentSupportModel>();
            schoolYearSops = new List<SchoolYearSopModel>();
            schoolYearResourceSupportDocuments = new List<SchoolYearResourceSupportDocumentModel>();
            schoolYearScholarships = new List<SchoolYearScholarshipModel>();
            schoolYearAwards = new List<SchoolYearAwardModel>();
            schoolYearSanctions = new List<SchoolYearSanctionModel>();
            selfGovernments = new List<SchoolYearSelfGovernmentModel>();
            internationalMobilities = new List<SchoolYearInternationalMobilityModel>();
            preSchoolReadinessResults = new List<PreSchoolReadinessResultsSingleYearModel>();
            reassessments = new List<SubjectAssessmentsModel>();
        }


        public string schoolName { get; set; }
        public string schoolSettlement { get; set; }
        public string schoolMunicipality { get; set; }
        public string schoolRegion { get; set; }
        public string schoolDistrict { get; set; }
        public string studentName { get; set; }
        public string studentEGN { get; set; }
        public string studentLNC { get; set; }
        public string studentOtherIdentifier { get; set; }
        public string studentNationality { get; set; }
        public string studentBirthDate { get; set; }
        public string studentBirthPlace { get; set; }
        public string studentBirthMunicipality { get; set; }
        public string studentBirthDistrict { get; set; }
        public string studentAddress { get; set; }
        public string studentPhone { get; set; }
        public string contentDescription { get; set; }


        public List<StudentClassModel> studentClasses { get; set; }
        public List<DocumentModel> documents { get; set; }  
        public List<SchoolYearAssessmentsModel> schoolYearAssessments { get; set; }
        public List<SchoolYearExternalEvaluationsModel> schoolYearExternalEvaluations { get; set; }
        public List<SchoolYearAssessmentsModel> schoolYearSelfEduFormAsssessments { get; set; }
        public List<SchoolYearAbsencesModel> schoolYearAbsences { get; set; }
        public List<SchoolYearCommonPersonDevelopmentSupportModel> schoolYearCommonPersonDevelopmentSupports { get; set; }
        public List<SchoolYearAdditionalPersonDevelopmentSupportModel> schoolYearAdditionalPersonDevelopmentSupports { get; set; }
        public List<SchoolYearSopModel> schoolYearSops { get; set; }
        public List<SchoolYearResourceSupportDocumentModel> schoolYearResourceSupportDocuments { get; set; }
        public List<SchoolYearScholarshipModel> schoolYearScholarships { get; set; }
        public List<SchoolYearAwardModel> schoolYearAwards { get; set; }
        public List<SchoolYearSanctionModel> schoolYearSanctions { get; set; }
        public List<SchoolYearSelfGovernmentModel> selfGovernments { get; set; }
        public List<SchoolYearInternationalMobilityModel> internationalMobilities { get; set; }
        public List<PreSchoolReadinessResultsSingleYearModel> preSchoolReadinessResults { get; set; }
        public List<SubjectAssessmentsModel> reassessments { get; set; }
    }
}
