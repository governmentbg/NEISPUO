namespace MON.Report.Service.Diploma.E2023
{
    using MON.DataAccess;
    using MON.Report.Model;
    using MON.Report.Model.Diploma;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MON.Shared;
    using System.ComponentModel;

    public class Diploma_3_54_BReportService : DiplomaReportBaseService<Diploma_3_54_BReportService>
    {
        public Diploma_3_54_BReportService(DbReportServiceDependencies<Diploma_3_54_BReportService> dependencies)
            : base(dependencies)
        {
        }

        public override object LoadReport(IDictionary<string, object> parameters)
        {
            DiplomaModel model = base.LoadReport(parameters) as DiplomaModel;
            if (model == null) return model;
            int diplomaId = GetIdAsInt(parameters);

            List<int?> excludedSubjectTypeIds = new List<int?>() { 101, 109 };

            var json = JsonConvert.SerializeObject(model);
            Diploma_3_54_BModel diploma = JsonConvert.DeserializeObject<Diploma_3_54_BModel>(json);

            diploma.RecognizedProfessionalSkills = FunctionsExtension.TryGetValue(diploma.DynamicData, "recognizedProfessionalSkills");
            diploma.DOSRegulationNo = FunctionsExtension.TryGetValue(diploma.DynamicData, "dosRegulationNo");
            diploma.DOSRegulationDate = FunctionsExtension.TryGetValue(diploma.DynamicData, "dosRegulationDate").ToNullableDateTime()?.ToString("dd.MM.yyyy"); ;
            diploma.DOSRegulationStateGazetteNo = FunctionsExtension.TryGetValue(diploma.DynamicData, "dosRegulationStateGazetteNo");
            diploma.DOSRegulationStateGazetteDate = FunctionsExtension.TryGetValue(diploma.DynamicData, "dosRegulationStateGazetteDate").ToNullableDateTime()?.ToString("dd.MM.yyyy"); ;

            var competencies = FunctionsExtension.TryGetValue(diploma.DynamicData, "recognizedKnowledge")?.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            diploma.Competencies = competencies != null ? competencies.Select(i => new Competency()
            {
                Code = i.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(),
                Name = String.Join(' ', i.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1))
            }).ToList() : new List<Competency>();

            // Секция ДИПрПК, временен вариант с търсене по име
            BasicDocumentPart stateExamPart = _db.BasicDocumentParts.FirstOrDefault(i => i.Name == "ДИ за признаване на квалификация" && i.BasicDocumentId == diploma.BasicDocumentId);
            diploma.StateExamBasicDocumentPartId = stateExamPart?.Id;
            diploma.StateExamQualificationGrades = diploma.Grades
                .Where(i => i.DocumentPartId == diploma.StateExamBasicDocumentPartId)
                .ToList();

            if (diplomaId != -1)
            {
                Diploma dbDiploma = _db.Diplomas.FirstOrDefault(i => i.Id == diplomaId);
                diploma.StateExamQualificationGradeText = dbDiploma?.StateExamQualificationGradeText;
                diploma.StateExamQualificationGrade = dbDiploma?.StateExamQualificationGrade;
            }

            if (diploma.StateExamQualificationGradeText != null && diploma.StateExamQualificationGrade != null)
            {
                diploma.StateExamQualificationGrades = new List<DiplomaGradeModel>()
                {
                    new DiplomaGradeModel()
                    {
                        SubjectName = " ",
                        GradeText = diploma.StateExamQualificationGradeText,
                        Grade = diploma.StateExamQualificationGrade,
                        DocumentPartId = (int)diploma.StateExamBasicDocumentPartId,
                    }
                };
            }

            diploma.Grades = diploma.Grades.Where(w => !excludedSubjectTypeIds.Contains(w.SubjectTypeId)).ToList();

            return diploma;
        }
    }
}
