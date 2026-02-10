using Microsoft.EntityFrameworkCore;
using MON.Report.Model;
using MON.Report.Model.Diploma;
using MON.Report.Model.Enums;
using MON.Shared;
using MON.Shared.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MON.Report.Service.Diploma.E2024
{
    public class Diploma_R_2ReportService : DiplomaReportBaseService<Diploma_R_2ReportService>
    {
        public Diploma_R_2ReportService(DbReportServiceDependencies<Diploma_R_2ReportService> dependencies)
            : base(dependencies)
        {
        }

        public override object LoadReport(IDictionary<string, object> parameters)
        {
            DiplomaModel model = (DiplomaModel)base.LoadReport(parameters);
            var json = JsonConvert.SerializeObject(model);
            Diploma_R_2Model diploma = JsonConvert.DeserializeObject<Diploma_R_2Model>(json);

            int diplomaId = GetIdAsInt(parameters);
            var dbDiploma = _db.Diplomas.FirstOrDefault(i => i.Id == diplomaId);

            diploma.LevelOfGraduation = FunctionsExtension.TryGetValue(diploma.DynamicData, "levelOfGraduation");
            diploma.IssuedBy = FunctionsExtension.TryGetValue(diploma.DynamicData, "issuedBy");
            diploma.ExternalDocNumber = FunctionsExtension.TryGetValue(diploma.DynamicData, "externalDocNumber");
            var externalDocDateString = FunctionsExtension.TryGetValue(diploma.DynamicData, "externalDocDate");
            if (DateTime.TryParse(externalDocDateString, out var externalDocDate))
            {
                diploma.ExternalDocDate = externalDocDate.ToString("dd.MM.yyyy г.");
            };
            diploma.RUO = _db.Regions.FirstOrDefault(i => i.RegionId == dbDiploma.RuoRegId)?.Name;
            diploma.PersonFullIdentifier = $"{(diploma.PersonalIdType == (int)PersonalIdTypeEnum.EGN ? "ЕГН" : (diploma.PersonalIdType == (int)PersonalIdTypeEnum.LNCH ? "ЛНЧ" : "ИДН"))} {diploma.PersonalId}" +
                $" {(diploma.Person.BirthDate != null ? "рожд. дата " + diploma.Person.BirthDate : "")} гражданство {diploma.Nationality}";
            return diploma;

            return diploma;
        }
    }
}
