using Microsoft.EntityFrameworkCore;
using MON.Report.Model;
using MON.Report.Model.Diploma;
using MON.Shared;
using MON.Shared.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MON.Report.Service.Diploma.E2023
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

            List<int> PrintedSubjectIds = new List<int>() { 1, 2, 36, 18, 13, 27, 28, 25, 29, 30, 31, 72, 45 };

            var json = JsonConvert.SerializeObject(model);
            Diploma_3_30Model diploma = JsonConvert.DeserializeObject<Diploma_3_30Model>(json);

            //Взимаме всички PartId - та и Code - ове за текущата диплома по BasicDocumentId
            Dictionary<int, string> basicDocumentPartCategoriesCollection = _db.BasicDocumentParts
                .Where(w => w.BasicDocumentId == diploma.BasicDocumentId)
                 .Select(s => new { s.Id, s.Code })
                 .OrderBy(o => o.Id)
                 .ToDictionary(td => td.Id, td => td.Code);

            List<int> mandatoryPartIds = basicDocumentPartCategoriesCollection
                .Where(w => w.Value == ((int)BasicDocumentPartCategoryEnum.MandatoryPart).ToString())
                .Select(s => s.Key).ToList();

            List<int> electivePartIds = basicDocumentPartCategoriesCollection
                .Where(w => w.Value == ((int)BasicDocumentPartCategoryEnum.ElectivePart).ToString())
                .Select(s => s.Key).ToList();

            List<int> optionalPartIds = basicDocumentPartCategoriesCollection
                .Where(w => w.Value == ((int)BasicDocumentPartCategoryEnum.OptionalPart).ToString())
                .Select(s => s.Key).ToList();

            int? nvoPartId = GetBasicDocumentPartCategory(diploma.BasicDocumentId, BasicDocumentPartCategoryEnum.NVOPart);



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

            List<int> foreignLanguageIds = _db.Fls.Where(i => i.Flid > 0).Select(i => i.Flid).ToList();

            diploma.FLMultiGrade = new MultiYearDiplomaGradeModel();

            List<DiplomaGradeModel> flGrade = diploma.Grades
                .Where(i => i.DocumentPartId == GetBasicDocumentPartCategory(diploma.BasicDocumentId, BasicDocumentPartCategoryEnum.MandatoryPart)
                && i.SubjectId != null
                && foreignLanguageIds.Contains(i.SubjectId.Value))
                .OrderBy(i => i.Position)
                .ToList();

            diploma.FLMultiGrade.SubjectId = flGrade.FirstOrDefault()?.SubjectId;
            diploma.FLMultiGrade.SubjectName = flGrade.FirstOrDefault()?.SubjectName;

            diploma.MandatoryMultiGrades =
                (from s in diploma.Grades
                 where mandatoryPartIds.Contains(s.DocumentPartId)
                 && basicDocumentPartCategoriesCollection.ContainsValue(((int)BasicDocumentPartCategoryEnum.MandatoryPart).ToString())
                 group s by new { s.SubjectId, s.SubjectName } into g
                 select new MultiYearDiplomaGradeModel()
                 {
                     DocumentPartName = g.FirstOrDefault().DocumentPartDescription,
                     SubjectId = g.Key.SubjectId,
                     SubjectName = g.Key.SubjectName,
                     Grades = g.ToList(),
                     DocumentPartId = g.FirstOrDefault().DocumentPartId
                 }).ToList();

            diploma.FLMultiGrade = diploma.MandatoryMultiGrades.FirstOrDefault(i => i.SubjectId == diploma.FLMultiGrade?.SubjectId);

            diploma.MandatoryAdditionalMultiGrades = diploma.MandatoryMultiGrades
                .Where(i => (i.SubjectId == null || (!PrintedSubjectIds.Contains(i.SubjectId.Value) && diploma.FLMultiGrade?.SubjectId != i.SubjectId.Value))).ToList();

            List<MultiYearDiplomaGradeModel> electiveMultiGrades =
                (from s in diploma.Grades
                 where electivePartIds.Contains(s.DocumentPartId)
                 && basicDocumentPartCategoriesCollection.ContainsValue(((int)BasicDocumentPartCategoryEnum.ElectivePart).ToString())
                 group s by new { s.SubjectId, s.SubjectName } into g
                 select new MultiYearDiplomaGradeModel()
                 {
                     Position = g.ToList().Min(i => i.Position) ?? 0,
                     DocumentPartName = g.FirstOrDefault().DocumentPartDescription,
                     SubjectId = g.Key.SubjectId,
                     SubjectName = g.Key.SubjectName,
                     Grades = g.ToList(),
                     DocumentPartId = g.FirstOrDefault().DocumentPartId,
                 }).OrderBy(i => i.Position).ToList();

            List<MultiYearDiplomaGradeModel> facultyMultiGrades =
                (from s in diploma.Grades
                 where optionalPartIds.Contains(s.DocumentPartId)
                 && basicDocumentPartCategoriesCollection.ContainsValue(((int)BasicDocumentPartCategoryEnum.OptionalPart).ToString())
                 group s by new { s.SubjectId, s.SubjectName } into g
                 select new MultiYearDiplomaGradeModel()
                 {
                     Position = g.ToList().Min(i => i.Position) ?? 0,
                     DocumentPartName = g.FirstOrDefault().DocumentPartDescription,
                     SubjectId = g.Key.SubjectId,
                     SubjectName = g.Key.SubjectName,
                     Grades = g.ToList(),
                     DocumentPartId = g.FirstOrDefault().DocumentPartId
                 }).OrderBy(i => i.Position).ToList();

            diploma.NonMandatoryMultiGrades = electiveMultiGrades.Concat(facultyMultiGrades).ToList();

            diploma.OldInstitutionTown = FunctionsExtension.TryGetValue(diploma.DynamicData, "oldInstitutionTown");
            diploma.OldInstitution = FunctionsExtension.TryGetValue(diploma.DynamicData, "oldInstitution");

            return diploma;
        }
    }
}
