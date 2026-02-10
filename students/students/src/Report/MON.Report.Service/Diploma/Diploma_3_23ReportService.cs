using MON.Report.Model;
using MON.Report.Model.Diploma;
using MON.Shared.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace MON.Report.Service.Diploma
{
    public class Diploma_3_23ReportService : DiplomaReportBaseService<Diploma_3_23ReportService>
    {
        public Diploma_3_23ReportService(DbReportServiceDependencies<Diploma_3_23ReportService> dependencies)
            : base(dependencies)
        {

        }

        public override object LoadReport(IDictionary<string, object> parameters)
        {
            DiplomaModel model = base.LoadReport(parameters) as DiplomaModel;
            int diplomaId = GetIdAsInt(parameters);

            var json = JsonConvert.SerializeObject(model);
            Diploma_3_23Model diploma = JsonConvert.DeserializeObject<Diploma_3_23Model>(json);
            
            if (diplomaId != -1)
            {
                diploma.MandatoryBasicDocumentPartId = GetBasicDocumentPartCategory(diploma.BasicDocumentId, BasicDocumentPartCategoryEnum.MandatoryPart);
                diploma.ElectiveBasicDocumentPartId = GetBasicDocumentPartCategory(diploma.BasicDocumentId, BasicDocumentPartCategoryEnum.ElectivePart);
                diploma.OptionalBasicDocumentPartId = GetBasicDocumentPartCategory(diploma.BasicDocumentId, BasicDocumentPartCategoryEnum.OptionalPart);
            }

            FillFLGrades(diploma);
            FillMandatoryAndNonMandatoryGrades(diploma);

            diploma.ElectiveGrades = diploma.Grades.Where(i => i.DocumentPartId == diploma.ElectiveBasicDocumentPartId).ToList();
            diploma.OptionalGrades = diploma.Grades.Where(i => i.DocumentPartId == diploma.OptionalBasicDocumentPartId).ToList();
            diploma.MandatoryGradesWithoutFL = diploma.MandatoryGrades.Where(i => i?.SubjectId == null || i.SubjectId < 100 || i.SubjectId > 150).ToList();

            diploma.BasicClassRomeNameNext = _db.BasicClasses.FirstOrDefault(i => i.BasicClassId == diploma.BasicClass + 1)?.RomeName;

            return diploma;
        }
    }
}