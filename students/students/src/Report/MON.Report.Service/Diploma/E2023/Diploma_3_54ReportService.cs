using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MON.DataAccess;
using MON.Report.Model;
using MON.Report.Model.Diploma;
using MON.Shared.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace MON.Report.Service.Diploma.E2023
{
    public class Diploma_3_54ReportService : DiplomaReportBaseService<Diploma_3_54ReportService>
    {
        public Diploma_3_54ReportService(DbReportServiceDependencies<Diploma_3_54ReportService> dependencies)
            : base(dependencies)
        {
        }

        public override object LoadReport(IDictionary<string, object> parameters)
        {
            DiplomaModel model = (DiplomaModel)base.LoadReport(parameters);

            List<int?> excludedSubjectTypeIds = new List<int?>() { 101, 109 };

            var json = JsonConvert.SerializeObject(model);
            Diploma_3_54Model diploma = JsonConvert.DeserializeObject<Diploma_3_54Model>(json);

            diploma.MandatoryBasicDocumentPartId = GetBasicDocumentPartCategory(diploma.BasicDocumentId, BasicDocumentPartCategoryEnum.MandatoryPart);
            diploma.StateExamBasicDocumentPartId = GetBasicDocumentPartCategory(diploma.BasicDocumentId, BasicDocumentPartCategoryEnum.ZIPProfPart); //в този случай това е ДИППК

            diploma.StateExamQualificationGrades = diploma.Grades
                .Where(i => i.DocumentPartId == diploma.StateExamBasicDocumentPartId)
                .ToList();

            int diplomaId = GetIdAsInt(parameters);
            if (diplomaId != -1)
            {
                var dbDiploma = _db.Diplomas.FirstOrDefault(i => i.Id == diplomaId);
                diploma.StateExamQualificationGradeText = dbDiploma?.StateExamQualificationGradeText;
                diploma.StateExamQualificationGrade = dbDiploma?.StateExamQualificationGrade == 0.00m ? null : dbDiploma?.StateExamQualificationGrade;
            }

            diploma.OldInstitutionTown = FunctionsExtension.TryGetValue(diploma.DynamicData, "oldInstitutionTown");
            diploma.OldInstitution = FunctionsExtension.TryGetValue(diploma.DynamicData, "oldInstitution");

            diploma.Grades = diploma.Grades
                .Where(i => i.DocumentPartId != diploma.StateExamBasicDocumentPartId && !excludedSubjectTypeIds.Contains(i.SubjectTypeId))
                .OrderBy(i => i.DocumentPartCode)
                .ToList();

            return diploma;
        }
    }

}
