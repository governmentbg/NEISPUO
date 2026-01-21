using MON.Report.Model;
using MON.Report.Model.Diploma;
using MON.Report.Service.Diploma;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace MON.Report.Service.Validation
{
    public class Validation_3_102ReportService : DiplomaReportBaseService<Validation_3_102ReportService>
    {
        public Validation_3_102ReportService(DbReportServiceDependencies<Validation_3_102ReportService> dependencies)
            : base(dependencies)
        {

        }

        public override object LoadReport(IDictionary<string, object> parameters)
        {
            DiplomaModel model = base.LoadReport(parameters) as DiplomaModel;
            if (model == null) return model;
            int diplomaId = GetIdAsInt(parameters);

            var json = JsonConvert.SerializeObject(model);
            Diploma_3_102Model diploma = JsonConvert.DeserializeObject<Diploma_3_102Model>(json);

            diploma.MainGrade = diploma.Grades.FirstOrDefault();

            var secondaryEducationDiploma = GetAdditionalDocument(diplomaId);
            diploma.SecondaryEducationDiploma = secondaryEducationDiploma;
            diploma.Session = FunctionsExtension.TryGetValue(diploma.DynamicData, "session");

            return diploma;
        }
    }
}
