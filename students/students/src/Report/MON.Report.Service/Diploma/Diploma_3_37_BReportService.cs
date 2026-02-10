namespace MON.Report.Service.Diploma
{
    using MON.Report.Model;
    using MON.Report.Model.Diploma;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using MON.Shared;

    public class Diploma_3_37_BReportService : DiplomaReportBaseService<Diploma_3_37_BReportService>
    {
        public Diploma_3_37_BReportService(DbReportServiceDependencies<Diploma_3_37_BReportService> dependencies)
            : base(dependencies)
        {
         
        }


        public override object LoadReport(IDictionary<string, object> parameters)
        {
            DiplomaModel model = base.LoadReport(parameters) as DiplomaModel;
            if (model == null) return model;
            int diplomaId = GetIdAsInt(parameters);

            var json = JsonConvert.SerializeObject(model);
            Diploma_3_37_BModel diploma = JsonConvert.DeserializeObject<Diploma_3_37_BModel>(json);

            diploma.RecognizedProfessionalSkills = FunctionsExtension.TryGetValue(diploma.DynamicData, "recognizedProfessionalSkills");
            diploma.DOSRegulationNo = FunctionsExtension.TryGetValue(diploma.DynamicData, "dosRegulationNo");
            diploma.DOSRegulationDate = FunctionsExtension.TryGetValue(diploma.DynamicData, "dosRegulationDate").ToNullableDateTime()?.ToString("dd.MM.yyyy"); ;
            diploma.DOSRegulationStateGazetteNo = FunctionsExtension.TryGetValue(diploma.DynamicData, "dosRegulationStateGazetteNo");
            diploma.DOSRegulationStateGazetteDate = FunctionsExtension.TryGetValue(diploma.DynamicData, "dosRegulationStateGazetteDate").ToNullableDateTime()?.ToString("dd.MM.yyyy"); ;

            diploma.ExamGrade = Convert.ToDecimal(FunctionsExtension.TryGetValue(diploma.DynamicData, "examGrade"));
            diploma.ExamGradeText = FunctionsExtension.TryGetValue(diploma.DynamicData, "examGradeText");

            diploma.ValidationOrderNumber = FunctionsExtension.TryGetValue(diploma.DynamicData, "validationOrderNumber");
            diploma.ValidationOrderDate = FunctionsExtension.TryGetValue(diploma.DynamicData, "validationOrderDate").ToNullableDateTime()?.ToString("dd.MM.yyyy");

            diploma.AmmendmentOrderNumber = FunctionsExtension.TryGetValue(diploma.DynamicData, "ammendmentOrderNumber");
            diploma.AmmendmentOrderDate = FunctionsExtension.TryGetValue(diploma.DynamicData, "ammendmentOrderDate").ToNullableDateTime()?.ToString("dd.MM.yyyy");
            diploma.RecognizedKnowledge = FunctionsExtension.TryGetValue(diploma.DynamicData, "recognizedKnowledge");

            return diploma;

        }
    }
}
