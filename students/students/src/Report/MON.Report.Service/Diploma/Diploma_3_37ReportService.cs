namespace MON.Report.Service.Diploma
{
    using MON.Report.Model;
    using MON.Report.Model.Diploma;
    using MON.Shared.Enums;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

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

            diploma.MandatoryBasicDocumentPartId = GetBasicDocumentPartCategory(diploma.BasicDocumentId, BasicDocumentPartCategoryEnum.MandatoryPart);

            diploma.ProfEduReason = FunctionsExtension.TryGetValue(diploma.DynamicData, "profEduReason");
            diploma.CourseName = FunctionsExtension.TryGetValue(diploma.DynamicData, "courseName");
            diploma.CourseDuration = Convert.ToDecimal(FunctionsExtension.TryGetValue(diploma.DynamicData, "courseDuration"));

            diploma.OldInstitutionTown = FunctionsExtension.TryGetValue(diploma.DynamicData, "oldInstitutionTown");
            diploma.OldInstitution = FunctionsExtension.TryGetValue(diploma.DynamicData, "oldInstitution");

            return diploma;
        }
    }
}