namespace MON.Services.Implementations.DiplomaCode
{
    using MON.Shared.Enums;
    using System;
    using System.Collections.Generic;

    public class Diploma_3_54_Code : CodeService
    {
        public Diploma_3_54_Code(DbServiceDependencies<CodeService> dependencies,
           IServiceProvider serviceProvider)
           : base(dependencies, serviceProvider)
        {
            BasicClassIds.AddRange(new List<int> { 8, 9, 10, 11, 12 });
            ReassessmentTypeIds.AddRange(new List<int> { (int)ReassessmentTypeEnum.FirstHighSchoolStage, (int)ReassessmentTypeEnum.SecondHighSchoolStage });
        }
    }
}
