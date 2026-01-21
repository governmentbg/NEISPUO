using MON.Report.Model;
using MON.Report.Model.Diploma;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace MON.Report.Service.Diploma.E2025
{
    public class Diploma_3_30_AReportService : DiplomaReportBaseService<Diploma_3_30_AReportService>
    {
        public Diploma_3_30_AReportService(DbReportServiceDependencies<Diploma_3_30_AReportService> dependencies)
            : base(dependencies)
        {

        }

        public override object LoadReport(IDictionary<string, object> parameters)
        {
            DiplomaModel model = base.LoadReport(parameters) as DiplomaModel;
            if (model == null) return model;
            int diplomaId = GetIdAsInt(parameters);

            var json = JsonConvert.SerializeObject(model);
            Diploma_3_30_AModel diploma = JsonConvert.DeserializeObject<Diploma_3_30_AModel>(json);

            diploma.Original = GetAdditionalDocument(diplomaId);

            string currentHeaderName = "";
            foreach (var grade in diploma.Grades.OrderBy(i => i.Position).ThenBy(i => i.Id))
            {
                if (grade.GradeCategory == -1)
                {
                    currentHeaderName = grade.SubjectName;
                }
                grade.DocumentPartName = currentHeaderName;
            }

            diploma.MultiGrades =
                (from s in diploma.Grades
                 group s by new { s.SubjectId, s.SubjectName, s.SubjectTypeId, s.DocumentPartName } into g
                 select new MultiYearDiplomaGradeModel()
                 {
                     DocumentPartId = g.FirstOrDefault().DocumentPartId,
                     DocumentPartName = g.FirstOrDefault().DocumentPartName,
                     SubjectId = g.Key.SubjectId,
                     SubjectName = g.Key.SubjectName,
                     Grades = g.ToList(),
                     GradeCategory = g.FirstOrDefault().GradeCategory
                 }).ToList();

            diploma.OldInstitutionTown = FunctionsExtension.TryGetValue(diploma.DynamicData, "oldInstitutionTown");
            diploma.OldInstitution = FunctionsExtension.TryGetValue(diploma.DynamicData, "oldInstitution");

            return diploma;
        }

    }
}
