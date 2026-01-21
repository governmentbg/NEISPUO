namespace MON.Services.Implementations.DiplomaCode
{
    using MON.Models.Diploma;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    public class Diploma_3_44_1_Code : CodeService
    {
        private const int FirstEducationalLevelBasicDocumentId = 14;

        public Diploma_3_44_1_Code(DbServiceDependencies<CodeService> dependencies, IServiceProvider serviceProvider)
            : base(dependencies, serviceProvider)
        {
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
