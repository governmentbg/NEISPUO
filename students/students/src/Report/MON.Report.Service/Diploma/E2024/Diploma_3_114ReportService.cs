namespace MON.Report.Service.Diploma.E2024
{
    using MON.Report.Model;
    using MON.Report.Model.Diploma;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using MON.Shared;

    public class Diploma_3_114ReportService : DiplomaReportBaseService<Diploma_3_114ReportService>
    {
        public Diploma_3_114ReportService(DbReportServiceDependencies<Diploma_3_114ReportService> dependencies)
            : base(dependencies)
        {
        }

        public override object LoadReport(IDictionary<string, object> parameters)
        {
            DiplomaModel model = (DiplomaModel)base.LoadReport(parameters);
            int diplomaId = GetIdAsInt(parameters);

            var json = JsonConvert.SerializeObject(model);
            Diploma_3_114Model diploma = JsonConvert.DeserializeObject<Diploma_3_114Model>(json);
            diploma.Enactment = FunctionsExtension.TryGetValue(diploma.DynamicData, "enactment");
            diploma.Graduated = FunctionsExtension.TryGetValue(diploma.DynamicData, "graduated");
            diploma.StateGazetteNo = FunctionsExtension.TryGetValue(diploma.DynamicData, "stateGazetteNo");
            diploma.StateGazetteDate = FunctionsExtension.TryGetValue(diploma.DynamicData, "stateGazetteDate").ToNullableDateTime()?.ToString("dd.MM.yyyy"); ;

            diploma.Ammendment = FunctionsExtension.TryGetValue(diploma.DynamicData, "ammendment");
            diploma.Certification = FunctionsExtension.TryGetValue(diploma.DynamicData, "certification");

            diploma.Level = FunctionsExtension.TryGetValue(diploma.DynamicData, "level");

            return diploma;
        }
    }
}