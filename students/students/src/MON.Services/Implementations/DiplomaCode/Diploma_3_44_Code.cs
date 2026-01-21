namespace MON.Services.Implementations.DiplomaCode
{
    using MON.Models.Diploma;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using MON.DataAccess;
    using MON.Shared.Enums;

    public class Diploma_3_44_Code : CodeService
    {
        private const int FirstEducationalLevelBasicDocumentId = 14;

        public Diploma_3_44_Code(DbServiceDependencies<CodeService> dependencies,
            IServiceProvider serviceProvider)
            : base(dependencies, serviceProvider)
        {
            BasicClassIds.AddRange(new List<int> { 11, 12 });
            ReassessmentTypeIds.AddRange(new List<int> { (int)ReassessmentTypeEnum.SecondHighSchoolStage });
        }

        // Автоматичното зареждане отпада.
        // https://github.com/Neispuo/students/issues/1374
        // Връзка между дубликат и оригинален документ #1374
        //public override async Task FillAdditionalDocuments(DiplomaCreateModel model)
        //{
        //    await FillAdditionalDocuments(model, FirstEducationalLevelBasicDocumentId);
        //}
    }
}
