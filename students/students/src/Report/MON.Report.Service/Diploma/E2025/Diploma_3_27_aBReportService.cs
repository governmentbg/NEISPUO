namespace MON.Report.Service.Diploma.E2025
{
    using MON.Report.Model;
    using MON.Report.Model.Diploma;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;

    public class Diploma_3_27_aBReportService : DiplomaReportBaseService<Diploma_3_27_aBReportService>
    {
        public Diploma_3_27_aBReportService(DbReportServiceDependencies<Diploma_3_27_aBReportService> dependencies) :
            base(dependencies)
        {
        }

        public override object LoadReport(IDictionary<string, object> parameters)
        {
            DiplomaModel model = (DiplomaModel)base.LoadReport(parameters);

            var json = JsonConvert.SerializeObject(model);
            DiplomaDuplicateModel diploma = JsonConvert.DeserializeObject<DiplomaDuplicateModel>(json);

            int diplomaId = GetIdAsInt(parameters);

            diploma.Original = GetAdditionalDocument(diplomaId);

            return diploma;
        }
    }
}
