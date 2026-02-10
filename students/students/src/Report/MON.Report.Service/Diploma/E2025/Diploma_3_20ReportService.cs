using MON.Report.Model;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using MON.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace MON.Report.Service.Diploma.E2025
{
    public class Diploma_3_20ReportService : DiplomaReportBaseService<Diploma_3_20ReportService>
    {
        public Diploma_3_20ReportService(DbReportServiceDependencies<Diploma_3_20ReportService> dependencies)
            : base(dependencies)
        {

        }

        public override object LoadReport(IDictionary<string, object> parameters)
        {
            DiplomaModel diploma = base.LoadReport(parameters) as DiplomaModel;

            int diplomaId = GetIdAsInt(parameters);
            string basicClassRomeName = (
                from d in _db.Diplomas
                where d.Id == diplomaId
                select d.BasicClass.RomeName).FirstOrDefault();


            ////Зареждане на клас -> BasicClassRomeName чрез DocumentPartId в базата са BasicClassId = 7, BasicClassId = 8
            //int? basicDocumentPartId = diploma.Grades.Select(i => i.DocumentPartId).Distinct().FirstOrDefault();
            //var basicDocumentPart = _db.BasicDocumentParts.Include(i => i.BasicClassNavigation).FirstOrDefault(i => i.Id == basicDocumentPartId);

            //if (basicDocumentPart != null)
            //{
            //    diploma.BasicClassRomeName = basicDocumentPart.BasicClassNavigation.RomeName;
            //}

            diploma.BasicClassRomeName = basicClassRomeName;
            diploma.OldInstitutionTown = FunctionsExtension.TryGetValue(diploma.DynamicData, "oldInstitutionTown");
            diploma.OldInstitution = FunctionsExtension.TryGetValue(diploma.DynamicData, "oldInstitution");

            return diploma;
        }
    }
}
