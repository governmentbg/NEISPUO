namespace MON.Report.Service.Diploma.E2024
{
    using MON.Report.Model;
    using MON.Report.Model.Diploma;
    using Newtonsoft.Json;
    using System.Linq;
    using System.Collections.Generic;
    using MON.DataAccess;
    using MON.Shared.Enums;

    public class Diploma_3_22_AReportService : DiplomaReportBaseService<Diploma_3_22_AReportService>
    {
        public Diploma_3_22_AReportService(DbReportServiceDependencies<Diploma_3_22_AReportService> dependencies)
            : base(dependencies)
        {

        }

        public override object LoadReport(IDictionary<string, object> parameters)
        {
            DiplomaModel model = base.LoadReport(parameters) as DiplomaModel;
            int diplomaId = GetIdAsInt(parameters);

            var json = JsonConvert.SerializeObject(model);
            Diploma_3_22_AModel diploma= JsonConvert.DeserializeObject<Diploma_3_22_AModel>(json);

            diploma.MandatoryBasicDocumentPartId = GetBasicDocumentPartCategory(diploma.BasicDocumentId, BasicDocumentPartCategoryEnum.MandatoryPart);
            diploma.ZIPProfBasicDocumentPartId = GetBasicDocumentPartCategory(diploma.BasicDocumentId, BasicDocumentPartCategoryEnum.ZIPProfPart);
            diploma.ZIPNoProfBasicDocumentPartId = GetBasicDocumentPartCategory(diploma.BasicDocumentId, BasicDocumentPartCategoryEnum.ZIPNoProfPart);
            diploma.SIPBasicDocumentPartId = GetBasicDocumentPartCategory(diploma.BasicDocumentId, BasicDocumentPartCategoryEnum.SIPPart);

            var original = GetAdditionalDocument(diplomaId);
            diploma.Original = original;

            FillFLGrades(diploma);
            FillMandatoryAndNonMandatoryGrades(diploma);

            diploma.OldInstitutionTown = FunctionsExtension.TryGetValue(diploma.DynamicData, "oldInstitutionTown");
            diploma.OldInstitution = FunctionsExtension.TryGetValue(diploma.DynamicData, "oldInstitution");

            return diploma;
        }
    }
}
