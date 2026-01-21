namespace MON.Services.Implementations.DiplomaCode
{
    using MON.Models.Diploma;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Threading.Tasks;

    public class Diploma_3_30_Code : CodeService
    {
        public Diploma_3_30_Code(DbServiceDependencies<CodeService> dependencies,
            IServiceProvider serviceProvider)
            : base(dependencies, serviceProvider)
        {
            BasicClassIds.AddRange(new List<int> { 5, 6, 7 });
        }
        
    }
}
