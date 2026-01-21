namespace MON.Report.Service.Diploma
{
    using Microsoft.EntityFrameworkCore;
    using MON.Report.Model;
    using System.Collections.Generic;
    using System.Linq;
    using MON.Report.Model.Diploma;
    using Newtonsoft.Json;
    using MON.Shared.Enums;

    public class Diploma_3_22ReportService : DiplomaReportBaseService<Diploma_3_22ReportService>
    {
        public Diploma_3_22ReportService(DbReportServiceDependencies<Diploma_3_22ReportService> dependencies)
            : base(dependencies)
        {

        }

        public override object LoadReport(IDictionary<string, object> parameters)
        {
            DiplomaModel model = base.LoadReport(parameters) as DiplomaModel;

            List<int> PrintedSubjectIds = new List<int>() { 1, 2, 17, 45, 35, 36, 12, 13, 14, 15, 5, 16, 28, 25, 29, 30, 31 };

            var json = JsonConvert.SerializeObject(model);
            Diploma_3_22Model diploma = JsonConvert.DeserializeObject<Diploma_3_22Model>(json);

            diploma.MandatoryBasicDocumentPartId = GetBasicDocumentPartCategory(diploma.BasicDocumentId, BasicDocumentPartCategoryEnum.MandatoryPart);
            diploma.ZIPProfBasicDocumentPartId = GetBasicDocumentPartCategory(diploma.BasicDocumentId, BasicDocumentPartCategoryEnum.ZIPProfPart);
            diploma.ZIPNoProfBasicDocumentPartId = GetBasicDocumentPartCategory(diploma.BasicDocumentId, BasicDocumentPartCategoryEnum.ZIPNoProfPart);
            diploma.SIPBasicDocumentPartId = GetBasicDocumentPartCategory(diploma.BasicDocumentId, BasicDocumentPartCategoryEnum.SIPPart);

            FillFLGrades(diploma);
            FillMandatoryAndNonMandatoryGrades(diploma);

            List<int> foreignLanguageIds = _db.Fls.Where(i => i.Flid > 0).Select(i => i.Flid).ToList();
            diploma.MandatoryAdditionalGrades = diploma.MandatoryGrades.Where(i => i.ExternalEvaluationTypeId == null && (i.SubjectId == null || (!PrintedSubjectIds.Contains(i.SubjectId.Value) && !foreignLanguageIds.Contains(i.SubjectId.Value)))).ToList();

            diploma.OldInstitutionTown = FunctionsExtension.TryGetValue(diploma.DynamicData, "oldInstitutionTown");
            diploma.OldInstitution = FunctionsExtension.TryGetValue(diploma.DynamicData, "oldInstitution");

            return diploma;
        }
    }
}
