using Microsoft.EntityFrameworkCore;
using MON.Report.Model;
using MON.Report.Model.Diploma;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace MON.Report.Service.Diploma
{
    public class Diploma_3_30ReportService : DiplomaReportBaseService<Diploma_3_30ReportService>
    {
        public Diploma_3_30ReportService(DbReportServiceDependencies<Diploma_3_30ReportService> dependencies)
            : base(dependencies)
        {

        }

        public override object LoadReport(IDictionary<string, object> parameters)
        {
            DiplomaModel model = base.LoadReport(parameters) as DiplomaModel;
            if (model == null) return model;
            List<int> MandatoryBasicDocumentPartIds = new List<int>() { 121, 122, 123 };
            int MandatoryBasicDocumentPartIdLastClass = 123;
            List<int> ElectiveBasicDocumentPartIds = new List<int>() { 125, 126, 127 };
            List<int> FacultyBasicDocumentPartIds = new List<int>() { 128, 129, 130 };
            List<int> PrintedSubjectIds = new List<int>() { 1, 2, 36, 18, 13, 27, 28, 25, 29, 30, 31, 72, 45 };

            var json = JsonConvert.SerializeObject(model);
            Diploma_3_30Model diploma = JsonConvert.DeserializeObject<Diploma_3_30Model>(json);

            diploma.MultiGrades =
                (from s in diploma.Grades
                 group s by new { s.SubjectId, s.SubjectName } into g
                 select new MultiYearDiplomaGradeModel()
                 {
                     DocumentPartId = g.FirstOrDefault().DocumentPartId,
                     DocumentPartName = g.FirstOrDefault().DocumentPartName,
                     SubjectId = g.Key.SubjectId,
                     SubjectName = g.Key.SubjectName,
                     Grades = g.ToList()
                 }).ToList();

            var foreignLanguageIds = _db.Fls.Where(i => i.Flid > 0).Select(i => i.Flid).ToList();

            diploma.FLMultiGrade = new MultiYearDiplomaGradeModel();

            var flGrade = diploma.Grades.Where(i => i.DocumentPartId == MandatoryBasicDocumentPartIdLastClass
                && i.SubjectId != null
                && foreignLanguageIds.Contains(i.SubjectId.Value))
                .OrderBy(i => i.Position)
                .ToList();
            diploma.FLMultiGrade.SubjectId = flGrade.FirstOrDefault()?.SubjectId;
            diploma.FLMultiGrade.SubjectName = flGrade.FirstOrDefault()?.SubjectName;

            diploma.MandatoryMultiGrades =
                (from s in diploma.Grades
                 where MandatoryBasicDocumentPartIds.Contains(s.DocumentPartId)
                 group s by new { s.SubjectId, s.SubjectName } into g
                 select new MultiYearDiplomaGradeModel()
                 {
                     DocumentPartName = g.FirstOrDefault().DocumentPartDescription,
                     SubjectId = g.Key.SubjectId,
                     SubjectName = g.Key.SubjectName,
                     Grades = g.ToList()
                 }).ToList();

            diploma.FLMultiGrade = diploma.MandatoryMultiGrades.FirstOrDefault(i => i.SubjectId == diploma.FLMultiGrade?.SubjectId);
            diploma.MandatoryAdditionalMultiGrades = diploma.MandatoryMultiGrades
                .Where(i => (i.SubjectId == null || (!PrintedSubjectIds.Contains(i.SubjectId.Value) && diploma.FLMultiGrade?.SubjectId != i.SubjectId.Value))).ToList();

            var electiveMultiGrades =
                (from s in diploma.Grades
                 where ElectiveBasicDocumentPartIds.Contains(s.DocumentPartId)
                 group s by new { s.SubjectId, s.SubjectName, s.SubjectTypeId } into g
                 select new MultiYearDiplomaGradeModel()
                 {
                     DocumentPartName = g.FirstOrDefault().DocumentPartDescription,
                     SubjectId = g.Key.SubjectId,
                     SubjectName = g.Key.SubjectName,
                     Grades = g.ToList()
                 }).ToList();

            var facultyMultiGrades =
                (from s in diploma.Grades
                 where FacultyBasicDocumentPartIds.Contains(s.DocumentPartId)
                 group s by new { s.SubjectId, s.SubjectName } into g
                 select new MultiYearDiplomaGradeModel()
                 {
                     DocumentPartName = g.FirstOrDefault().DocumentPartDescription,
                     SubjectId = g.Key.SubjectId,
                     SubjectName = g.Key.SubjectName,
                     Grades = g.ToList()
                 }).ToList();

            diploma.NonMandatoryMultiGrades = electiveMultiGrades.Concat(facultyMultiGrades).ToList();

            diploma.OldInstitutionTown = FunctionsExtension.TryGetValue(diploma.DynamicData, "oldInstitutionTown");
            diploma.OldInstitution = FunctionsExtension.TryGetValue(diploma.DynamicData, "oldInstitution");

            return diploma;
        }
    }
}
