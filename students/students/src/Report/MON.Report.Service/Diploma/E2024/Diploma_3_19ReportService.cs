namespace MON.Report.Service.Diploma.E2024
{
    using MON.Report.Model;
    using Newtonsoft.Json;
    using System.Linq;
    using System.Collections.Generic;
    using MON.DataAccess;
    using Microsoft.EntityFrameworkCore;
    using MON.Report.Model.Diploma;

    public class Diploma_3_19ReportService : DiplomaReportBaseService<Diploma_3_19ReportService>
    {
        public Diploma_3_19ReportService(DbReportServiceDependencies<Diploma_3_19ReportService> dependencies)
            : base(dependencies)
        {
        }

        public override object LoadReport(IDictionary<string, object> parameters)
        {
            DiplomaModel model = base.LoadReport(parameters) as DiplomaModel;

            if (model == null) return model;
            
            var json = JsonConvert.SerializeObject(model);
            Diploma_3_19Model diploma = JsonConvert.DeserializeObject<Diploma_3_19Model>(json);

            diploma.PreSchoolOrganization = FunctionsExtension.TryGetValue(diploma.DynamicData, "preSchoolOrganization");

            return diploma;
        }
    }
}
