using MON.Report.Model;
using MON.Report.Model.Diploma;
using MON.Shared.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MON.Report.Service.Diploma.E2025
{
    public class Diploma_3_25ReportService : DiplomaReportBaseService<Diploma_3_25ReportService>
    {
        public Diploma_3_25ReportService(DbReportServiceDependencies<Diploma_3_25ReportService> dependencies)
            : base(dependencies)
        {

        }

        public override object LoadReport(IDictionary<string, object> parameters)
        {
            DiplomaModel model = base.LoadReport(parameters) as DiplomaModel;
            int diplomaId = GetIdAsInt(parameters);
            var json = JsonConvert.SerializeObject(model);
            Diploma_3_25Model diploma = JsonConvert.DeserializeObject<Diploma_3_25Model>(json);

            diploma.MandatoryBasicDocumentPartId = GetBasicDocumentPartCategory(diploma.BasicDocumentId, BasicDocumentPartCategoryEnum.MandatoryPart);
            diploma.ElectiveBasicDocumentPartId = GetBasicDocumentPartCategory(diploma.BasicDocumentId, BasicDocumentPartCategoryEnum.ElectivePart);
            diploma.OptionalBasicDocumentPartId = GetBasicDocumentPartCategory(diploma.BasicDocumentId, BasicDocumentPartCategoryEnum.OptionalPart);

            FillFLGrades(diploma);
            FillMandatoryAndNonMandatoryGrades(diploma);

            //Заменено е с филтър по SubjectTypeId в MandatoryGradesWithoutFL по-долу.
            //Пречи на предмети с код под 200, попадащи едновременно в ЗУЧ и ИУЧ
            //List<int?> existingMandatorySubjectIds = new List<int?>() { 1, 2, 27, 30, 31, 45, 71, 72, 11 }

            //предмети, които фабрично са отпечатани на бланката
            List<int?> existingMandatorySubjectIds = new List<int?>() { 1, 2, 11, 27, 30, 31, 45, 71, 72 };

            diploma.ElectiveGrades = diploma.Grades.Where(i => i.DocumentPartId == diploma.ElectiveBasicDocumentPartId).ToList();
            diploma.OptionalGrades = diploma.Grades.Where(i => i.DocumentPartId == diploma.OptionalBasicDocumentPartId).ToList();
            diploma.MandatoryGradesWithoutFL = diploma.MandatoryGrades.Where(i => ((i?.SubjectId == null || i.SubjectId < 100 || i.SubjectId > 150) && !existingMandatorySubjectIds.Contains(i.SubjectId) && i.BasicSubjectType == (int)BasicSubjectTypeEnum.CompulsoryCourses)).ToList();

            //ВРЕМЕННО РЕШЕНИЕ - МОЖЕ БИ ЩЕ ТРЯБВА ДА СЕ ПРОМЕНИ
            foreach (DiplomaGradeModel grade in diploma.MandatoryGradesWithoutFL)
            {
                grade.DocumentPartName = "";
            }

            List<DiplomaGradeModel> diplomaFilteredGrades = new List<DiplomaGradeModel>();

            diplomaFilteredGrades.AddRange(diploma.MandatoryGradesWithoutFL.Where(d => d.DocumentPartName != null));
            diplomaFilteredGrades.AddRange(diploma.ElectiveGrades);
            diplomaFilteredGrades.AddRange(diploma.OptionalGrades);

            if (diplomaFilteredGrades.Count > 0 || diplomaFilteredGrades != null)
            {
                diploma.DiplomaFilteredGrades = diplomaFilteredGrades;
            }
            //ВРЕМЕННО РЕШЕНИЕ - МОЖЕ БИ ЩЕ ТРЯБВА ДА СЕ ПРОМЕНИ

            return diploma;
        }
    }
}