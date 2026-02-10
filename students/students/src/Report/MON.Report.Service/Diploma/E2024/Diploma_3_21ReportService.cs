namespace MON.Report.Service.Diploma.E2024
{
    using MON.Report.Model.Diploma;
    using MON.Report.Model;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using MON.Services.Infrastructure.Validators;
    using MON.Shared.Extensions;

    public class Diploma_3_21ReportService : DiplomaReportBaseService<Diploma_3_21ReportService>
    {
        public Diploma_3_21ReportService(DbReportServiceDependencies<Diploma_3_21ReportService> dependencies)
            : base(dependencies)
        {

        }
    }
}
