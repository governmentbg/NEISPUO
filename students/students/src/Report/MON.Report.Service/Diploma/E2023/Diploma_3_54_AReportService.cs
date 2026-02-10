namespace MON.Report.Service.Diploma.E2023
{   
    using MON.Report.Model;
    using MON.Report.Model.Diploma;
    using Newtonsoft.Json;
    using System.Linq;
    using System.Collections.Generic;
    using MON.DataAccess;

    public class Diploma_3_54_AReportService : DiplomaReportBaseService<Diploma_3_54_AReportService>
    {
        public Diploma_3_54_AReportService(DbReportServiceDependencies<Diploma_3_54_AReportService> dependencies)
            : base(dependencies)
        {
        }

        public override object LoadReport(IDictionary<string, object> parameters)
        {
            List<int?> excludedSubjectTypeIds = new List<int?>() { 101, 109 };

            DiplomaModel model = (DiplomaModel)base.LoadReport(parameters);
            int diplomaId = GetIdAsInt(parameters);

            var json = JsonConvert.SerializeObject(model);
            Diploma_3_54_AModel diploma = JsonConvert.DeserializeObject<Diploma_3_54_AModel>(json);

            BasicDocumentPart stateExamPart = _db.BasicDocumentParts.FirstOrDefault(i => i.Name == "ДИППК" && i.BasicDocumentId == diploma.BasicDocumentId);
            diploma.StateExamBasicDocumentPartId = stateExamPart?.Id;

            if (diplomaId != -1)
            {
                var dbDiploma = _db.Diplomas.FirstOrDefault(i => i.Id == diplomaId);
                diploma.StateExamQualificationGradeText = dbDiploma?.StateExamQualificationGradeText;
                diploma.StateExamQualificationGrade = dbDiploma?.StateExamQualificationGrade == 0.00m ? null : dbDiploma?.StateExamQualificationGrade;
            }

            diploma.OldInstitutionTown = FunctionsExtension.TryGetValue(diploma.DynamicData, "oldInstitutionTown");
            diploma.OldInstitution = FunctionsExtension.TryGetValue(diploma.DynamicData, "oldInstitution");

            diploma.StateExamQualificationGrades = diploma.Grades
                .Where(i => i.DocumentPartId == diploma.StateExamBasicDocumentPartId)
                .ToList();

            diploma.Grades = diploma.Grades
                .Where(i => i.DocumentPartId != diploma.StateExamBasicDocumentPartId && !excludedSubjectTypeIds.Contains(i.SubjectTypeId))
                .OrderBy(i => i.DocumentPartCode)
                .ToList();

            diploma.Original = GetAdditionalDocument(diplomaId);
            return diploma;
        }
    }
}
