namespace MON.Report.Service.Diploma.E2025
{

    using MON.DataAccess;
    using MON.Report.Model;
    using MON.Report.Model.Diploma;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MON.Shared;
    public class Diploma_3_54_1ReportService : DiplomaReportBaseService<Diploma_3_54_1ReportService>
    {
        public Diploma_3_54_1ReportService(DbReportServiceDependencies<Diploma_3_54_1ReportService> dependencies) :
            base(dependencies)
        {
        }

        public override object LoadReport(IDictionary<string, object> parameters)
        {
            DiplomaModel model = base.LoadReport(parameters) as DiplomaModel;
            if (model == null) return model;
            int diplomaId = GetIdAsInt(parameters);

            List<int?> excludedSubjectTypeIds = new List<int?>() { 101, 109 };

            var json = JsonConvert.SerializeObject(model);
            Diploma_3_54_1Model diploma = JsonConvert.DeserializeObject<Diploma_3_54_1Model>(json);

            

            diploma.Grades = diploma.Grades.Where(w => !excludedSubjectTypeIds.Contains(w.SubjectTypeId)).ToList();

            return diploma;
        }
    }
}
