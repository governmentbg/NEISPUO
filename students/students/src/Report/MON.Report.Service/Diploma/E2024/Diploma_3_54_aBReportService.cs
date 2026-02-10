namespace MON.Report.Service.Diploma.E2024
{
    using MON.DataAccess;
    using MON.Report.Model;
    using MON.Report.Model.Diploma;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MON.Shared;
    using MON.Shared.Enums;

    public class Diploma_3_54_aBReportService : DiplomaReportBaseService<Diploma_3_54_aBReportService>
    {
        public Diploma_3_54_aBReportService(DbReportServiceDependencies<Diploma_3_54_aBReportService> dependencies)
            : base(dependencies)
        {
        }

        public override object LoadReport(IDictionary<string, object> parameters)
        {
            DiplomaModel model = (DiplomaModel)base.LoadReport(parameters);

            var json = JsonConvert.SerializeObject(model);
            Diploma_3_54_aBModel diploma = JsonConvert.DeserializeObject<Diploma_3_54_aBModel>(json);
            int diplomaId = GetIdAsInt(parameters);

            diploma.MandatoryBasicDocumentPartId = GetBasicDocumentPartCategory(diploma.BasicDocumentId, BasicDocumentPartCategoryEnum.MandatoryPart);

            string[] competencies = FunctionsExtension.TryGetValue(diploma.DynamicData, "competencies")?.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (competencies != null && competencies.Length > 0)
            {
                diploma.Competencies = competencies.Select(i => new Competency()
                {
                    Code = i.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(),
                    Name = String.Join(' ', i.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1))
                }).ToList();
            }
            else
            {
                diploma.Competencies = new List<Competency>()
                {
                    new Competency("-", "-")
                };
            }

            diploma.StateExamBasicDocumentPartId = GetBasicDocumentPartCategory(diploma.BasicDocumentId, BasicDocumentPartCategoryEnum.MandatoryPart); // Секция ДИПрПК
            diploma.StateExamQualificationGrades = diploma.Grades 
                .Where(i => i.DocumentPartId == diploma.StateExamBasicDocumentPartId)
                .ToList();

            if (diplomaId != -1)
            {
                Diploma dbDiploma = _db.Diplomas.FirstOrDefault(i => i.Id == diplomaId);
                diploma.StateExamQualificationGradeText = dbDiploma?.StateExamQualificationGradeText;
                diploma.StateExamQualificationGrade = dbDiploma?.StateExamQualificationGrade;
            }

            if (diploma.StateExamQualificationGradeText != null && diploma.StateExamQualificationGrade != 0.00m)
            {
                diploma.StateExamQualificationGrades = new List<DiplomaGradeModel>()
                {
                    new DiplomaGradeModel()
                    {
                        SubjectName = "Обща оценка за ДИПрПК",
                        GradeText = diploma.StateExamQualificationGradeText,
                        Grade = diploma.StateExamQualificationGrade,
                        DocumentPartId = (int)diploma.StateExamBasicDocumentPartId,
                    }
                };
            }

            return diploma;
        }
    }
}
