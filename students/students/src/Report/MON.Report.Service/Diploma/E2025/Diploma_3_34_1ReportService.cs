namespace MON.Report.Service.Diploma.E2025
{
    using MON.Report.Model;
    using MON.Report.Model.Diploma;
    using MON.Shared.Enums;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;

    public class Diploma_3_34_1ReportService : DiplomaReportBaseService<Diploma_3_34_1ReportService>
    {
        public Diploma_3_34_1ReportService(DbReportServiceDependencies<Diploma_3_34_1ReportService> dependencies) :
            base(dependencies)
        {
        }

        public override object LoadReport(IDictionary<string, object> parameters)
        {
            DiplomaModel diploma = (DiplomaModel)base.LoadReport(parameters);

            diploma.MandatoryBasicDocumentPartId = GetBasicDocumentPartCategory(diploma.BasicDocumentId, BasicDocumentPartCategoryEnum.MandatoryPart);

            return diploma;
        }
    }
}
