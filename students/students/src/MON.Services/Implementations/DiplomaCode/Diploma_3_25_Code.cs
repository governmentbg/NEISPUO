namespace MON.Services.Implementations.DiplomaCode
{
    using System;
    using System.Collections.Generic;

    public class Diploma_3_25_Code : CodeService
    {
        public Diploma_3_25_Code(DbServiceDependencies<CodeService> dependencies,
           IServiceProvider serviceProvider)
           : base(dependencies, serviceProvider)
        {
            BasicClassIds.AddRange(new List<int> { 4 });
        }
    }
}
