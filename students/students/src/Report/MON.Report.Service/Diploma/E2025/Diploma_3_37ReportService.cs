namespace MON.Report.Service.Diploma.E2025
{
    using MON.DataAccess;
    using MON.Report.Model;
    using MON.Report.Model.Diploma;
    using MON.Shared.Enums;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Diploma_3_37ReportService : DiplomaReportBaseService<Diploma_3_37ReportService>
    {
        public Diploma_3_37ReportService(DbReportServiceDependencies<Diploma_3_37ReportService> dependencies) :
            base(dependencies)
        {
        }
        
        public override object LoadReport(IDictionary<string, object> parameters)
        {
            DiplomaModel model = base.LoadReport(parameters) as DiplomaModel;

            if (model == null) return model;

            var json = JsonConvert.SerializeObject(model);
            Diploma_3_37Model diploma = JsonConvert.DeserializeObject<Diploma_3_37Model>(json);

            int diplomaId = GetIdAsInt(parameters);

            diploma.MandatoryBasicDocumentPartId = GetBasicDocumentPartCategory(diploma.BasicDocumentId, BasicDocumentPartCategoryEnum.MandatoryPart);

            diploma.ProfEduReason = FunctionsExtension.TryGetValue(diploma.DynamicData, "profEduReason");
            diploma.CourseName = FunctionsExtension.TryGetValue(diploma.DynamicData, "courseName");
            diploma.CourseDuration = Convert.ToDecimal(FunctionsExtension.TryGetValue(diploma.DynamicData, "courseDuration"));

            diploma.OldInstitutionTown = FunctionsExtension.TryGetValue(diploma.DynamicData, "oldInstitutionTown");
            diploma.OldInstitution = FunctionsExtension.TryGetValue(diploma.DynamicData, "oldInstitution");

            diploma.StateExamBasicDocumentPartId = GetBasicDocumentPartCategory(diploma.BasicDocumentId, BasicDocumentPartCategoryEnum.ZIPProfPart); //Квалификационни изпити
            diploma.StateExamQualificationGrades = diploma.Grades
                .Where(i => i.DocumentPartId == diploma.StateExamBasicDocumentPartId)
                .ToList();

            if (diplomaId != -1)
            {
                Diploma dbDiploma = _db.Diplomas.FirstOrDefault(i => i.Id == diplomaId);

                diploma.Gpa = dbDiploma?.Gpa;
                diploma.GpaText = dbDiploma?.Gpatext;
            }

            if (diploma.GpaText != null && diploma.Gpa != 0.00m)
            {
                diploma.StateExamQualificationGrades = new List<DiplomaGradeModel>()
                {
                    new DiplomaGradeModel()
                    {
                        SubjectName = "-",
                        GradeText = diploma.GpaText,
                        Grade = diploma.Gpa,
                    }
                };
            }

            return diploma;
        }
    }
}