using Microsoft.EntityFrameworkCore;
using MON.Report.Model;
using MON.Report.Model.Diploma;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace MON.Report.Service.Diploma.E2024
{
    public class Diploma_3_31_AReportService : DiplomaReportBaseService<Diploma_3_31_AReportService>
    {
        public Diploma_3_31_AReportService(DbReportServiceDependencies<Diploma_3_31_AReportService> dependencies)
            : base(dependencies)
        {
        }

        public override object LoadReport(IDictionary<string, object> parameters)
        {
            List<int> PrintedSubjectIds = new List<int>() { 1, 2, 5, 13, 17, 18, 25, 28, 29, 36, 45 };
            DiplomaModel model = (DiplomaModel)base.LoadReport(parameters);
            model.MandatoryBasicDocumentPartId = 22;
            int diplomaId = GetIdAsInt(parameters);

            var json = JsonConvert.SerializeObject(model);
            DiplomaDuplicateModel diploma = JsonConvert.DeserializeObject<DiplomaDuplicateModel>(json);

            FillMandatoryAndNonMandatoryGrades(diploma);
            FillFLGrades(diploma);
            var foreignLanguageIds = _db.Fls.Where(i => i.Flid > 0).Select(i => i.Flid).ToList();
            diploma.MandatoryAdditionalGrades = diploma.MandatoryGrades.Where(i => i.ExternalEvaluationTypeId == null && (i.SubjectId == null || (!PrintedSubjectIds.Contains(i.SubjectId.Value) && !foreignLanguageIds.Contains(i.SubjectId.Value)))).ToList();

            diploma.Original = GetAdditionalDocument(diplomaId);

            diploma.OldInstitutionTown = FunctionsExtension.TryGetValue(diploma.DynamicData, "oldInstitutionTown");
            diploma.OldInstitution = FunctionsExtension.TryGetValue(diploma.DynamicData, "oldInstitution");

            return diploma;
        }
    }
}
