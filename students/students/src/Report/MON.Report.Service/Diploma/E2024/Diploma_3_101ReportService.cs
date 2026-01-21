using MON.Report.Model;
using MON.Report.Model.Diploma;
using MON.Report.Service.Diploma;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace MON.Report.Service.Diploma.E2024
{
    public class Diploma_3_101ReportService : DiplomaReportBaseService<Diploma_3_101ReportService>
    {
        public Diploma_3_101ReportService(DbReportServiceDependencies<Diploma_3_101ReportService> dependencies)
            : base(dependencies)
        {

        }

        public override object LoadReport(IDictionary<string, object> parameters)
        {
            DiplomaModel model = base.LoadReport(parameters) as DiplomaModel;
            if (model == null) return model;
            int diplomaId = GetIdAsInt(parameters);

            var json = JsonConvert.SerializeObject(model);
            Diploma_3_101Model diploma = JsonConvert.DeserializeObject<Diploma_3_101Model>(json);

            diploma.MainGrade = diploma.Grades.FirstOrDefault();

            diploma.Session = FunctionsExtension.TryGetValue(diploma.DynamicData, "session");

            return diploma;
        }
    }
}
