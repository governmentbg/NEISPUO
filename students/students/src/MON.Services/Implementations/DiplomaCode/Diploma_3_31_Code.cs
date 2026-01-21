namespace MON.Services.Implementations.DiplomaCode
{
    using MON.Models.Diploma;
    using MON.Services.Interfaces;
    using MON.Shared.Enums;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using MON.Shared;
    using MON.Shared.Extensions;
    using Newtonsoft.Json.Serialization;
    using Microsoft.EntityFrameworkCore;

    public class Diploma_3_31_Code : CodeService
    {
        public Diploma_3_31_Code(DbServiceDependencies<CodeService> dependencies, IServiceProvider serviceProvider)
            : base(dependencies, serviceProvider)
        {
            BasicClassIds.AddRange(new List<int> { 8, 9, 10 });
            ReassessmentTypeIds.AddRange(new List<int> { (int)ReassessmentTypeEnum.FirstHighSchoolStage });
        }

        public override async Task AfterFillGrades(DiplomaCreateModel model)
        {
            ExpandoObject modelContents = model.Contents.IsNullOrWhiteSpace()
                ? new ExpandoObject()
                : JsonConvert.DeserializeObject<ExpandoObject>(model.Contents, new ExpandoObjectConverter());

            // Ако е положен НВО по Информационни технологии и точките са 50 или над 50, в нивото на дигитални компетентности трябва да се отпечата "самостоятелно владеене", т.е. ITLevel-ът да стане 4
            var nvoPart = model.BasicDocumentTemplate.Parts.FirstOrDefault(p => p.HasExternalEvaluationLimit && p.ExternalEvaluationTypes.Contains((int)ExternalEvaluationTypeEnum.NVO_10));
            if (nvoPart != null)
            {
                // Информационни технологии е с ID = 36
                var itSubject = nvoPart.Subjects.FirstOrDefault(s => s.SubjectId == 36);
                if (itSubject != null && itSubject.NvoPoints >= 50)
                {
                    AddOrUpdate(modelContents, "itLevel", 4);
                }

                var flSubjectIds = await _context.Fls.Where(i => i.IsValid.HasValue && i.IsValid.Value).Select(i => i.Flid).ToListAsync();
                var flSubject = nvoPart.Subjects.FirstOrDefault(s => s.SubjectId.HasValue && flSubjectIds.Contains(s.SubjectId.Value));
                // По наредба 11, само в случая когато по НВО има над 60 точки се вписва
                if (flSubject != null && flSubject.NvoPoints >= 60)
                {
                    // Има чужд език в НВО-то
                    AddOrUpdate(modelContents, "flLevel", flSubject.FlLevel);
                }

            }

            model.Contents = modelContents.ToJson(new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

    }
}
