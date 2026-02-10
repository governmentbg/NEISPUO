namespace MON.Report.Service.Diploma.E2025
{
    using MON.Report.Model.Diploma;
    using MON.Report.Model;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using MON.DataAccess;
    using System;
    using MON.Shared.Enums;
    using MON.Shared;

    public class Diploma_3_44_2ReportService : DiplomaReportBaseService<Diploma_3_44_2ReportService>
    {
        public Diploma_3_44_2ReportService(DbReportServiceDependencies<Diploma_3_44_2ReportService> dependencies) :
            base(dependencies)
        {
        }

        public override object LoadReport(IDictionary<string, object> parameters)
        {
            DiplomaModel model = (DiplomaModel)base.LoadReport(parameters);
            int diplomaId = GetIdAsInt(parameters);

            var json = JsonConvert.SerializeObject(model);
            Diploma_3_44_Foreign_Model diploma = JsonConvert.DeserializeObject<Diploma_3_44_Foreign_Model>(json);

            Diploma dbDiploma = _db.Diplomas.FirstOrDefault(x => x.Id == diplomaId);

            AdditionalDocumentModel firstHighSchoolLevel = (
                           from i in _db.DiplomaAdditionalDocuments
                           where i.DiplomaId == diplomaId
                           select new AdditionalDocumentModel()
                           {
                               IsValidation = i.BasicDocument.IsValidation,
                               Series = i.Series,
                               FactoryNumber = i.FactoryNumber,
                               InstitutionId = i.InstitutionId,
                               Institution = i.InstitutionName,
                               RegNumberTotal = i.RegistratioNumber,
                               RegNumberYear = i.RegistrationNumberYear,
                               RegDate = i.RegistrationDate != null ? i.RegistrationDate.Value.ToString("dd.MM.yyyy") : ""
                           }).FirstOrDefault();

            if (firstHighSchoolLevel?.InstitutionId != null)
            {
                Institution institution = _db.Institutions.FirstOrDefault(i => i.InstitutionId == firstHighSchoolLevel.InstitutionId);
                if (String.IsNullOrWhiteSpace(firstHighSchoolLevel.Institution))
                {
                    firstHighSchoolLevel.Institution = institution?.Name;
                }
            }

            diploma.SpecialityForeign = _db.Sppoospecialities.FirstOrDefault(i => i.SppoospecialityCode == diploma.SpecialityCode && i.IsValid == true)?.NameDe ?? null;
            diploma.ProfessionForeign = _db.Sppooprofessions.FirstOrDefault(i => i.SppooprofessionCode == diploma.ProfessionCode && i.IsValid == true)?.NameDe ?? null;
            diploma.Ministry = _db.Ministries.FirstOrDefault(i => i.Id == dbDiploma.MinistryId)?.NameDe;
            diploma.StateExamQualificationGradeText = GetDecimalGradeText(dbDiploma.StateExamQualificationGrade);
            diploma.StateExamQualificationGrade = dbDiploma.StateExamQualificationGrade;
            diploma.EduFormForeign = _db.EduForms.FirstOrDefault(i => i.ClassEduFormId == dbDiploma.EduFormId)?.NameDe;
            diploma.ClassType = _db.ClassTypes.FirstOrDefault(i => i.ClassTypeId == diploma.ClassTypeId)?.NameDe ?? null;
            diploma.GpaTextForeign = GetDecimalGradeText(dbDiploma.Gpa);
            diploma.FirstNameLatin = dbDiploma?.FirstNameLatin;
            diploma.MiddleNameLatin = dbDiploma?.MiddleNameLatin;
            diploma.LastNameLatin = dbDiploma?.LastNameLatin;
            diploma.FullNameLatin = $"{diploma.FirstNameLatin ?? ""} {diploma.MiddleNameLatin ?? ""} {diploma.LastNameLatin ?? ""}";

            foreach (DiplomaGradeModel grade in diploma.Grades)
            {
                Subject subject = _db.Subjects.FirstOrDefault(i => i.SubjectId == grade.SubjectId);
                if (!String.IsNullOrWhiteSpace(subject?.NameDe))
                {
                    grade.SubjectName = subject.NameDe;
                }

                grade.GradeText = GetDecimalGradeText(grade.Grade);
            }

            diploma.Person.BirthPlaceTownForeign = FunctionsExtension.TryGetValue(diploma.DynamicData, "birthPlaceTownForeign");
            diploma.Person.BirthPlaceRegionForeign = FunctionsExtension.TryGetValue(diploma.DynamicData, "birthPlaceRegionForeign");
            diploma.Person.BirthPlaceMunicipalityForeign = FunctionsExtension.TryGetValue(diploma.DynamicData, "birthPlaceMunicipalityForeign");
            diploma.NationalityForeign = FunctionsExtension.TryGetValue(diploma.DynamicData, "nationalityForeign");
            diploma.OldInstitutionTown = FunctionsExtension.TryGetValue(diploma.DynamicData, "oldInstitutionTown");
            diploma.OldInstitution = FunctionsExtension.TryGetValue(diploma.DynamicData, "oldInstitution");
            diploma.InstitutionTownForeign = FunctionsExtension.TryGetValue(diploma.DynamicData, "institutionTownForeign");
            diploma.InstitutionRegionForeign = FunctionsExtension.TryGetValue(diploma.DynamicData, "institutionRegionForeign");
            diploma.InstitutionMunicipalityForeign = FunctionsExtension.TryGetValue(diploma.DynamicData, "institutionMunicipalityForeign");
            diploma.InstitutionLocalAreaForeign = FunctionsExtension.TryGetValue(diploma.DynamicData, "institutionLocalAreaForeign");
            diploma.InstitutionForeign = FunctionsExtension.TryGetValue(diploma.DynamicData, "institutionForeign");

            FillFLGrades(diploma);
            FillMandatoryAndNonMandatoryGrades(diploma);

            IEnumerable<int?> subjectTypeIdsForExternalEvaluation = diploma.Grades.Where(i => i.ExternalEvaluationTypeId == (int)ExternalEvaluationTypeEnum.DZI).Select(i => i.SubjectTypeId).Distinct();
            diploma.SubjectTypesDetails = new List<SubjectTypeDetail>();

            if (subjectTypeIdsForExternalEvaluation.Intersect(GlobalConstants.SubjectTypesOfGeneralEduSubject).Any())
            {
                diploma.SubjectTypesDetails.Add(new SubjectTypeDetail() { Name = "AF", Description = "als allgemeinbildendes Fach erlernt" });
            }
            if (subjectTypeIdsForExternalEvaluation.Intersect(GlobalConstants.SubjectTypesOfProfileSubject).Any())
            {
                diploma.SubjectTypesDetails.Add(new SubjectTypeDetail() { Name = "SF", Description = "als Schwerpunktfach erlernt" });
            }

            diploma.FirstHighSchoolLevel = firstHighSchoolLevel ?? new AdditionalDocumentModel();

            return diploma;
        }

        private string GetDecimalGradeText(decimal? gradeValue)
        {
            return gradeValue switch
            {
                var gv when gv < 3.5m => "Ausreichend",
                var gv when gv < 4.5m => "Gut",
                var gv when gv < 5.5m => "Sehr gut",
                var gv when gv >= 5.5m => "Ausgezeichnet",
                _ => string.Empty
            };
        }
    }
}