using Microsoft.EntityFrameworkCore;
using MON.DataAccess;
using MON.Report.Model;
using MON.Report.Model.Diploma;
using MON.Report.Model.Enums;
using MON.Shared;
using MON.Shared.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MON.Report.Service.Diploma.E2024
{
    public class Diploma_3_44ReportService : DiplomaReportBaseService<Diploma_3_44ReportService>
    {
        public Diploma_3_44ReportService(DbReportServiceDependencies<Diploma_3_44ReportService> dependencies)
            : base(dependencies)
        {
        }

        public override object LoadReport(IDictionary<string, object> parameters)
        {
            List<int> PrintedSubjectIds = new List<int>() { 1, 2, 17, 45 };

            DiplomaModel model = (DiplomaModel)base.LoadReport(parameters);

            var json = JsonConvert.SerializeObject(model);
            Diploma_3_44Model diploma = JsonConvert.DeserializeObject<Diploma_3_44Model>(json);

            int diplomaId = GetIdAsInt(parameters);

            // Зареждаме данни от базата, само ако не сме в дизайн режим
            if (diplomaId != -1)
            {
                diploma.MandatoryBasicDocumentPartId = GetBasicDocumentPartCategory(diploma.BasicDocumentId, BasicDocumentPartCategoryEnum.MandatoryPart);
                AdditionalDocumentModel firstHighSchoolLevel = (
                from i in _db.DiplomaAdditionalDocuments
                where i.DiplomaId == diplomaId
                select new AdditionalDocumentModel()
                {
                    IsValidation = i.BasicDocument.IsValidation,
                    // TODO: Проверяваме дали документът е с id=332. Засега това е единствения документ, който е признаване
                    IsRecognition = i.BasicDocumentId == 332,
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

                DataAccess.Diploma dbDiploma = _db.Diplomas.FirstOrDefault(i => i.Id == diplomaId);

                diploma.StateExamQualificationGradeText = dbDiploma.StateExamQualificationGradeText;
                diploma.StateExamQualificationGrade = dbDiploma.StateExamQualificationGrade;

                FillFLGrades(diploma);
                FillMandatoryAndNonMandatoryGrades(diploma);

                List<int> foreignLanguageIds = _db.Fls.Where(i => i.Flid > 0).Select(i => i.Flid).ToList();

                diploma.MandatoryAdditionalGrades = diploma.MandatoryGrades.Where(i => i.ExternalEvaluationTypeId == null
                && (i.SubjectId == null || (!PrintedSubjectIds.Contains(i.SubjectId.Value)
                && !foreignLanguageIds.Contains(i.SubjectId.Value)))).ToList();

                diploma.FirstHighSchoolLevel = firstHighSchoolLevel ?? new AdditionalDocumentModel();
                diploma.SubjectTypesDetails = new List<SubjectTypeDetail>();

                IEnumerable<int?> subjectTypeIdsForExternalEvaluation = diploma.Grades
                    .Where(i => i.ExternalEvaluationTypeId == (int)ExternalEvaluationTypeEnum.DZI && i.SubjectId != (int)SubjectsEnum.BEL).Select(i => i.SubjectTypeId).Distinct();

                if (subjectTypeIdsForExternalEvaluation.Intersect(GlobalConstants.SubjectTypesOfGeneralEduSubject).Any())
                {
                    diploma.SubjectTypesDetails.Add(new SubjectTypeDetail() { Name = "ООП", Description = "изучаван като общообразователен предмет" });
                }

                if (subjectTypeIdsForExternalEvaluation.Intersect(GlobalConstants.SubjectTypesOfProfileSubject).Any())
                {
                    diploma.SubjectTypesDetails.Add(new SubjectTypeDetail() { Name = "ПП", Description = "изучаван като профилиращ предмет" });
                }

                diploma.OldInstitutionTown = FunctionsExtension.TryGetValue(diploma.DynamicData, "oldInstitutionTown");
                diploma.OldInstitution = FunctionsExtension.TryGetValue(diploma.DynamicData, "oldInstitution");
            }

            return diploma;
        }
    }
}
