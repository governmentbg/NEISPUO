namespace MON.Report.Service.Diploma.E2024
{
    using Microsoft.EntityFrameworkCore;
    using MON.Report.Model;
    using MON.Shared.Enums;
    using System.Collections.Generic;
    using System.Linq;

    public class Diploma_3_103ReportService : DiplomaReportBaseService<Diploma_3_103ReportService>
    {
        public Diploma_3_103ReportService(DbReportServiceDependencies<Diploma_3_103ReportService> dependencies)
            : base(dependencies)
        {

        }

        public override object LoadReport(IDictionary<string, object> parameters)
        {
            DiplomaModel diploma = base.LoadReport(parameters) as DiplomaModel;

            diploma.MandatoryBasicDocumentPartId = GetBasicDocumentPartCategory(diploma.BasicDocumentId, BasicDocumentPartCategoryEnum.MandatoryPart);

            // Зареждане на оценките от НВО за 10-ми клас
            const string basicClassName = "10";

            List<DiplomaGradeModel> externalEvaluations = _db.ExternalEvaluationItems
                .AsNoTracking()
                .Where(x => x.ExternalEvaluation.PersonId == diploma.PersonId
                    && x.ExternalEvaluation.ExternalEvaluationType.BasicClass.Name == basicClassName)
                .Select(x => new DiplomaGradeModel
                {
                    SubjectName = x.Subject,
                    SubjectNameShort = x.Subject,
                    Points = x.Points
                })
                .ToList();

            diploma.ExtEvalByBasicClass = new Dictionary<string, List<DiplomaGradeModel>> {
                { basicClassName, new List<DiplomaGradeModel>() }
            };

            // В шаблона първо е "Български език и литература", второ "Математика" и третото е празно.
            var s1 = externalEvaluations.FirstOrDefault(x => x.SubjectName.Contains("български език", System.StringComparison.OrdinalIgnoreCase));
            var s2 = externalEvaluations.FirstOrDefault(x => x.SubjectName.Contains("математика", System.StringComparison.OrdinalIgnoreCase));
            var hash = new HashSet<string>(new[] { s1?.SubjectName ?? "", s2?.SubjectName ?? "" });

            diploma.ExtEvalByBasicClass[basicClassName].Add(s1);
            diploma.ExtEvalByBasicClass[basicClassName].Add(s2);
            diploma.ExtEvalByBasicClass[basicClassName].AddRange(externalEvaluations.Where(x => !hash.Contains(x.SubjectName)));


            var foreignLanguageIds = _db.Fls.Where(i => i.Flid > 0).Select(i => i.Flid).ToList();

            diploma.FLGrades = diploma.Grades.Where(i => i.DocumentPartId == diploma.MandatoryBasicDocumentPartId
                && i.SubjectId != null
                && foreignLanguageIds.Contains(i.SubjectId.Value)).ToList();

            diploma.MandatoryGrades = diploma.Grades.Where(i => i.DocumentPartId == diploma.MandatoryBasicDocumentPartId).ToList();
            diploma.NonMandatoryGrades = diploma.Grades.Where(i => i.DocumentPartId != diploma.MandatoryBasicDocumentPartId).ToList();

            diploma.OldInstitutionTown = FunctionsExtension.TryGetValue(diploma.DynamicData, "oldInstitutionTown");
            diploma.OldInstitution = FunctionsExtension.TryGetValue(diploma.DynamicData, "oldInstitution");

            return diploma;
        }
    }
}
