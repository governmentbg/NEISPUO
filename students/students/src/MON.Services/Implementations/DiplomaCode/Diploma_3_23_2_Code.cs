namespace MON.Services.Implementations.DiplomaCode
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Diploma_3_23_2_Code : CodeService
    {
        public Diploma_3_23_2_Code(DbServiceDependencies<CodeService> dependencies, IServiceProvider serviceProvider)
            : base(dependencies, serviceProvider)
        {
            //Класът вече се избира предварително от настройка в diploma.BasicDocument.BasicClasses
            //BasicClassIds.AddRange(new List<int> { 2 });
        }
    }
}
