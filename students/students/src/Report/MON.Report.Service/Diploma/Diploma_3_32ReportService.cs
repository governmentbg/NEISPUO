namespace MON.Report.Service.Diploma
{
    using Microsoft.EntityFrameworkCore;
    using MON.Report.Model;
    using MON.Shared.Enums;
    using System.Collections.Generic;
    using System.Linq;

    public class Diploma_3_32ReportService : DiplomaReportBaseService<Diploma_3_32ReportService>
    {
        public Diploma_3_32ReportService(DbReportServiceDependencies<Diploma_3_32ReportService> dependencies)
            : base(dependencies)
        {

        }

        public override object LoadReport(IDictionary<string, object> parameters)
        {
            List<int> PrintedSubjectIds = new List<int>() { 1, 2, 17, 45 };
            DiplomaModel diploma = base.LoadReport(parameters) as DiplomaModel;
            diploma.MandatoryBasicDocumentPartId = GetBasicDocumentPartCategory(diploma.BasicDocumentId, BasicDocumentPartCategoryEnum.MandatoryPart);

            FillFLGrades(diploma);
            FillMandatoryAndNonMandatoryGrades(diploma);
            var foreignLanguageIds = _db.Fls.Where(i => i.Flid > 0).Select(i => i.Flid).ToList();
            diploma.MandatoryAdditionalGrades = diploma.MandatoryGrades.Where(i => i.ExternalEvaluationTypeId == null && (i.SubjectId == null || (!PrintedSubjectIds.Contains(i.SubjectId.Value) && !foreignLanguageIds.Contains(i.SubjectId.Value)))).ToList();

            diploma.OldInstitutionTown = FunctionsExtension.TryGetValue(diploma.DynamicData, "oldInstitutionTown");
            diploma.OldInstitution = FunctionsExtension.TryGetValue(diploma.DynamicData, "oldInstitution");

            return diploma;
        }
    }
}
