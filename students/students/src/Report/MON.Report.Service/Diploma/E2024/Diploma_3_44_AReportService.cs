namespace MON.Report.Service.Diploma.E2024
{
    using MON.Report.Model;
    using MON.Report.Model.Diploma;
    using Newtonsoft.Json;
    using System.Linq;
    using System.Collections.Generic;

    public class Diploma_3_44_AReportService : DiplomaReportBaseService<Diploma_3_44_AReportService>
    {
        public Diploma_3_44_AReportService(DbReportServiceDependencies<Diploma_3_44_AReportService> dependencies)
            : base(dependencies)
        {
        }


        public override object LoadReport(IDictionary<string, object> parameters)
        {
            DiplomaModel model = (DiplomaModel)base.LoadReport(parameters);
            int diplomaId = GetIdAsInt(parameters);

            var json = JsonConvert.SerializeObject(model);
            Diploma_3_44_AModel diploma = JsonConvert.DeserializeObject<Diploma_3_44_AModel>(json);

            diploma.CommissionType = FunctionsExtension.TryGetValue(diploma.DynamicData, "commissionType");
            diploma.Qualification = FunctionsExtension.TryGetValue(diploma.DynamicData, "qualification");

            diploma.Original = GetAdditionalDocument(diplomaId);
            // Търсим само документ тип 3-31
            diploma.FirstHighSchoolLevel = GetAdditionalDocumentByType(diplomaId, new List<int?>() { 14 });
            // Ако оригиналният документ е 3-44, задължително трябва да има секция за първи гимназиален етап, като тя може и да е с "-"
            if (diploma.Original?.BasicDocumentId == 253)
            {
                if(diploma.FirstHighSchoolLevel == null)
                {
                    diploma.FirstHighSchoolLevel = new DiplomaDocumentOriginal()
                    {
                        Institution = "-",
                        Series = "-",
                        FactoryNumber = "-",
                        RegNumberTotal = "-",
                        RegNumberYear = "-",
                        GraduationType = "-"
                    };
                }
            }

            FillFLGrades(diploma);
            FillMandatoryAndNonMandatoryGrades(diploma);

            diploma.OldInstitutionTown = FunctionsExtension.TryGetValue(diploma.DynamicData, "oldInstitutionTown");
            diploma.OldInstitution = FunctionsExtension.TryGetValue(diploma.DynamicData, "oldInstitution");

            return diploma;
        }
    }
}
