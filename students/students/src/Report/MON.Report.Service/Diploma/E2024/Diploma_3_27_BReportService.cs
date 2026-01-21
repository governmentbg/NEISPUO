namespace MON.Report.Service.Diploma.E2024
{
    using MON.Report.Model.Diploma;
    using MON.Report.Model;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using MON.Shared;

    public class Diploma_3_27_BReportService : DiplomaReportBaseService<Diploma_3_27_BReportService>
    {
        public Diploma_3_27_BReportService(DbReportServiceDependencies<Diploma_3_27_BReportService> dependencies)
            : base(dependencies)
        {
        }

        public override object LoadReport(IDictionary<string, object> parameters)
        {
            DiplomaModel model = base.LoadReport(parameters) as DiplomaModel;
            if (model == null) return model;

            var json = JsonConvert.SerializeObject(model);
            Diploma_3_37_BModel diploma = JsonConvert.DeserializeObject<Diploma_3_37_BModel>(json);     

            diploma.ValidationOrderNumber = FunctionsExtension.TryGetValue(diploma.DynamicData, "validationOrderNumber");
            diploma.ValidationOrderDate = FunctionsExtension.TryGetValue(diploma.DynamicData, "validationOrderDate").ToNullableDateTime()?.ToString("dd.MM.yyyy");

            return diploma;
        }
    }
}
