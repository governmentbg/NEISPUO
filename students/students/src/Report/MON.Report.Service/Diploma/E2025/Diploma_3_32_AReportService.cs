namespace MON.Report.Service.Diploma.E2025
{
    using MON.Report.Model;
    using MON.Report.Model.Diploma;
    using Newtonsoft.Json;
    using System.Linq;
    using System.Collections.Generic;
    using MON.Shared.Enums;

    public class Diploma_3_32_AReportService : DiplomaReportBaseService<Diploma_3_32_AReportService>
    {
        public Diploma_3_32_AReportService(DbReportServiceDependencies<Diploma_3_32_AReportService> dependencies)
            : base(dependencies)
        {

        }

        public override object LoadReport(IDictionary<string, object> parameters)
        {
            List<int> PrintedSubjectIds = new List<int>() { 1, 2, 17, 45 };
            DiplomaModel model = (DiplomaModel)base.LoadReport(parameters);
            int diplomaId = GetIdAsInt(parameters);

            var json = JsonConvert.SerializeObject(model);
            DiplomaDuplicateModel diploma = JsonConvert.DeserializeObject<DiplomaDuplicateModel>(json);
            diploma.MandatoryBasicDocumentPartId = GetBasicDocumentPartCategory(diploma.BasicDocumentId, BasicDocumentPartCategoryEnum.MandatoryPart);

            diploma.Original = GetAdditionalDocument(diplomaId);

            FillFLGrades(diploma);
            FillMandatoryAndNonMandatoryGrades(diploma);
            var foreignLanguageIds = _db.Fls.Where(i => i.Flid > 0).Select(i => i.Flid).ToList();
            diploma.MandatoryAdditionalGrades = diploma.MandatoryGrades.Where(i => i.ExternalEvaluationTypeId == null && (i.SubjectId == null || (!PrintedSubjectIds.Contains(i.SubjectId.Value) && !foreignLanguageIds.Contains(i.SubjectId.Value)))).ToList();

            diploma.OldInstitutionTown = FunctionsExtension.TryGetValue(diploma.DynamicData, "oldInstitutionTown");
            diploma.OldInstitution = FunctionsExtension.TryGetValue(diploma.DynamicData, "oldInstitution");

            return diploma;
        }
    }
}