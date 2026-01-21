namespace MON.Services.Implementations.DiplomaCode
{
    using MON.Models.Diploma;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using MON.DataAccess;

    public class Diploma_Laureate_Code : CodeService
    {
        private const int HighSchoolBasicDocumentId = 253;

        public Diploma_Laureate_Code(DbServiceDependencies<CodeService> dependencies,
            IServiceProvider serviceProvider)
            : base(dependencies, serviceProvider)
        {
        }

        // Автоматичното зареждане отпада.
        // https://github.com/Neispuo/students/issues/1374
        // Връзка между дубликат и оригинален документ #1374
        //public override async Task FillAdditionalDocuments(DiplomaCreateModel model)
        //{
        //    await FillAdditionalDocuments(model, HighSchoolBasicDocumentId);
        //}
    }
}
