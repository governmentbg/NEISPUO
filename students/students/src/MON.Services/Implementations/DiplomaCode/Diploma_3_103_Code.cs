namespace MON.Services.Implementations.DiplomaCode
{
    using System;
    using System.Collections.Generic;

    public class Diploma_3_103_Code : CodeService
    {
        public Diploma_3_103_Code(DbServiceDependencies<CodeService> dependencies,
            IServiceProvider serviceProvider)
            : base(dependencies, serviceProvider)
        {
            // Todo: Това е удостоверение за завършен клас. Класът следва да се изббира предварително. Временно зареждаме само оценки за 12-ти клас.
            //Класът вече се избира предварително от настройка в diploma.BasicDocument.BasicClasses
            //BasicClassIds.AddRange(new List<int> { 12 });
        }
    }
}
