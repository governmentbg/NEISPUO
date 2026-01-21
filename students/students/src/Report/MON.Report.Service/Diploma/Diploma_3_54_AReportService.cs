namespace MON.Report.Service.Diploma
{
    using MON.Report.Model;
    using MON.Report.Model.Diploma;
    using Newtonsoft.Json;
    using System.Linq;
    using System.Collections.Generic;
    using MON.DataAccess;

    public class Diploma_3_54_AReportService : DiplomaReportBaseService<Diploma_3_54_AReportService>
    {
        public Diploma_3_54_AReportService(DbReportServiceDependencies<Diploma_3_54_AReportService> dependencies)
            : base(dependencies)
        {
        }

        public override object LoadReport(IDictionary<string, object> parameters)
        {
            DiplomaModel model = (DiplomaModel)base.LoadReport(parameters);
            int diplomaId = GetIdAsInt(parameters);

            var json = JsonConvert.SerializeObject(model);
            Diploma_3_54_AModel diploma = JsonConvert.DeserializeObject<Diploma_3_54_AModel>(json);

            BasicDocumentPart stateExamPart = _db.BasicDocumentParts.FirstOrDefault(i => i.Name == "ДИППК" && i.BasicDocumentId == diploma.BasicDocumentId);
            diploma.StateExamBasicDocumentPartId = stateExamPart?.Id;

            diploma.OldInstitutionTown = FunctionsExtension.TryGetValue(diploma.DynamicData, "oldInstitutionTown");
            diploma.OldInstitution = FunctionsExtension.TryGetValue(diploma.DynamicData, "oldInstitution");

            diploma.StateExamQualificationGrades = diploma.Grades
                .Where(i => i.DocumentPartId == diploma.StateExamBasicDocumentPartId)
                .ToList();

            diploma.Grades = diploma.Grades
                .Where(i => i.DocumentPartId != diploma.StateExamBasicDocumentPartId)
                .OrderBy(i => i.DocumentPartCode)
                .ToList();

            diploma.Original = GetAdditionalDocument(diplomaId);
            return diploma;
        }
    }
}
