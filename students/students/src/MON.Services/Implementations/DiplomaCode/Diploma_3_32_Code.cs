namespace MON.Services.Implementations.DiplomaCode
{
    using MON.Shared.Enums;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Diploma_3_32_Code : CodeService
    {
        public Diploma_3_32_Code(DbServiceDependencies<CodeService> dependencies, IServiceProvider serviceProvider)
            : base(dependencies, serviceProvider)
        {
            BasicClassIds.AddRange(new List<int> { 11, 12 });
            ReassessmentTypeIds.AddRange(new List<int> { (int)ReassessmentTypeEnum.SecondHighSchoolStage });
        }
    }
}
