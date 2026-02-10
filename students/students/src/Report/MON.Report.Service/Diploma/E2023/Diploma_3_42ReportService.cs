namespace MON.Report.Service.Diploma.E2023
{
    using MON.Report.Model.Diploma;
    using MON.Report.Model;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;

    public class Diploma_3_42ReportService : DiplomaReportBaseService<Diploma_3_42ReportService>
    {
        public Diploma_3_42ReportService(DbReportServiceDependencies<Diploma_3_42ReportService> dependencies)
            : base(dependencies)
        {
        }

        public override object LoadReport(IDictionary<string, object> parameters)
        {
            DiplomaModel model = base.LoadReport(parameters) as DiplomaModel;

            if (model == null) return model;

            var json = JsonConvert.SerializeObject(model);
            Diploma_3_42Model diploma = JsonConvert.DeserializeObject<Diploma_3_42Model>(json);

            diploma.GraduationCommissionMembers = diploma.GraduationCommissionMembers?.Take(6).ToList();
            diploma.Qualification = FunctionsExtension.TryGetValue(diploma.DynamicData, "qualification");

            return diploma;
        }
    }
}
