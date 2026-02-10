namespace MON.Report.Service.Diploma
{
    using MON.Report.Model.Diploma;
    using MON.Report.Model;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System;

    public class Diploma_3_22_1ReportService : DiplomaReportBaseService<Diploma_3_22_1ReportService>
    {
        public Diploma_3_22_1ReportService(DbReportServiceDependencies<Diploma_3_22_1ReportService> dependencies)
            : base(dependencies)
        {

        }


        public override object LoadReport(IDictionary<string, object> parameters)
        {
            DiplomaModel model = base.LoadReport(parameters) as DiplomaModel;
            if (model == null) return model;
            int diplomaId = GetIdAsInt(parameters);

            var json = JsonConvert.SerializeObject(model);
            Diploma_3_22_1Model diploma = JsonConvert.DeserializeObject<Diploma_3_22_1Model>(json);
            var originalDocument = GetAdditionalDocument(diplomaId);
            if (originalDocument?.BasicDocument != null)
            {
                if (originalDocument.BasicDocument.Contains("3-31"))
                {
                    diploma.HighSchoolLevel = "първи";
                }
                else if (originalDocument.BasicDocument.Contains("3-32"))
                {
                    diploma.HighSchoolLevel = "втори";
                }
                else
                {
                    diploma.HighSchoolLevel = "-";
                }
            }

            return diploma;
        }
    }
}