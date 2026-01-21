namespace MON.Report.Service.Diploma
{
    using Microsoft.EntityFrameworkCore;
    using MON.DataAccess;
    using MON.Report.Model;
    using MON.Report.Model.Diploma;
    using MON.Shared.Enums;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Diploma_3_34ReportService : DiplomaReportBaseService<Diploma_3_34ReportService>
    {
        public Diploma_3_34ReportService(DbReportServiceDependencies<Diploma_3_34ReportService> dependencies) :
            base(dependencies)
        {
        }

        public override object LoadReport(IDictionary<string, object> parameters)
        {
            DiplomaModel model = base.LoadReport(parameters) as DiplomaModel;

            if (model == null) return model;

            List<int> PrintedSubjectIds = new List<int>() { 1, 2, 17, 45, 35, 36, 12, 13, 14, 15, 5, 16, 28, 25, 29, 30, 31 };

            var json = JsonConvert.SerializeObject(model);
            Diploma_3_34Model diploma = JsonConvert.DeserializeObject<Diploma_3_34Model>(json);

            diploma.MandatoryBasicDocumentPartId = GetBasicDocumentPartCategory(diploma.BasicDocumentId, BasicDocumentPartCategoryEnum.MandatoryPart);
            diploma.ZIPNoProfBasicDocumentPartId = GetBasicDocumentPartCategory(diploma.BasicDocumentId, BasicDocumentPartCategoryEnum.ZIPNoProfPart);
            diploma.ZIPProfBasicDocumentPartId = GetBasicDocumentPartCategory(diploma.BasicDocumentId, BasicDocumentPartCategoryEnum.ZIPProfPart);
            diploma.SIPBasicDocumentPartId = GetBasicDocumentPartCategory(diploma.BasicDocumentId, BasicDocumentPartCategoryEnum.SIPPart);
            diploma.AdditionalDZIBasicDocumentPartId = GetBasicDocumentPartCategory(diploma.BasicDocumentId, BasicDocumentPartCategoryEnum.AdditionalDZIPart);
            diploma.MandatoryDZIBasicDocumentPartId = GetBasicDocumentPartCategory(diploma.BasicDocumentId, BasicDocumentPartCategoryEnum.MandatoryDZIPart);

            FillFLGrades(diploma);
            FillMandatoryAndNonMandatoryGrades(diploma);

            List<int> foreignLanguageIds = _db.Fls.Where(i => i.Flid > 0).Select(i => i.Flid).ToList();
            diploma.MandatoryAdditionalGrades = diploma.MandatoryGrades.Where(i => i.ExternalEvaluationTypeId == null && (i.SubjectId == null || (!PrintedSubjectIds.Contains(i.SubjectId.Value) && !foreignLanguageIds.Contains(i.SubjectId.Value)))).ToList();

            diploma.AdditionalDZI = diploma.Grades?.Where(i => i.DocumentPartId == diploma.AdditionalDZIBasicDocumentPartId).ToList();
            diploma.GraduationCommissionMembers = diploma.GraduationCommissionMembers?.Take(6).ToList();

            return diploma;
        }
    }
}
