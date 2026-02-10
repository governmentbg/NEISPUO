namespace MON.Services.Implementations.DiplomaCode
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Diploma_3_22A_Code : CodeService
    {
        public Diploma_3_22A_Code(DbServiceDependencies<CodeService> dependencies, IServiceProvider serviceProvider)
            : base(dependencies, serviceProvider)
        {
            BasicClassIds.AddRange(new List<int> { 8, 9, 10, 11, 12 });
        }
    }
}
