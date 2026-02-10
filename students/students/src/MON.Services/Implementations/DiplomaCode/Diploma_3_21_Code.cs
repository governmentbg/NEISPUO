namespace MON.Services.Implementations.DiplomaCode
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using MON.DataAccess;
    using MON.Models;
    using MON.Models.Diploma;
    using MON.Models.Enums.UserManagement;
    using MON.Models.UserManagement;
    using MON.Services.Interfaces;
    using MON.Shared;
    using MON.Shared.Extensions;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;
    using System;
    using System.Dynamic;
    using System.Linq;
    using System.Threading.Tasks;

    public class Diploma_3_21_Code : CodeService
    {
        public Diploma_3_21_Code(DbServiceDependencies<CodeService> dependencies,
            IServiceProvider serviceProvider)
            : base(dependencies, serviceProvider)
        {

        }

        public async override Task<string> AutoFillDynamicContent(int basicDocumentId, int? personId, string contents)
        {
            string data = await base.AutoFillDynamicContent(basicDocumentId, personId, contents);
            ExpandoObject model = data.IsNullOrWhiteSpace()
                ? new ExpandoObject()
                : JsonConvert.DeserializeObject<ExpandoObject>(data, new ExpandoObjectConverter());

            var grade = _context.FirstGradeResults.FirstOrDefault(i => i.PersonId == personId);
            if (grade != null)
            {
                if (grade.QualitativeGrade != null)
                {
                    base.AddIfNotExists(model, "gpaText", GradeUtils.GetQualitativeGradeText(grade.QualitativeGrade));
                }
                else if (grade.SpecialGrade != null)
                {
                    base.AddIfNotExists(model, "gpaText", GradeUtils.GetSpecialNeedsGradeText(grade.SpecialGrade));
                }
            }

            return model.ToJson(new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

    }
}
