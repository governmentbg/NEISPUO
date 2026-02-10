namespace MON.Services.Implementations.DiplomaCode
{
    using MON.Models.Diploma;
    using MON.Shared;
    using MON.Shared.Enums;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class Diploma_3_54A_Code : CodeService
    {
        // Id на оригиналния документ
        const int originalBasicDocumentId = 17;

        public Diploma_3_54A_Code(DbServiceDependencies<CodeService> dependencies, IServiceProvider serviceProvider)
            : base(dependencies, serviceProvider)
        {
            BasicClassIds.AddRange(new List<int> { 8, 9, 10, 11, 12 });
            ReassessmentTypeIds.AddRange(new List<int> { (int)ReassessmentTypeEnum.FirstHighSchoolStage, (int)ReassessmentTypeEnum.SecondHighSchoolStage, });
        }

        // Автоматичното зареждане отпада.
        // https://github.com/Neispuo/students/issues/1374
        // Връзка между дубликат и оригинален документ #1374
        //public override async Task FillAdditionalDocuments(DiplomaCreateModel model)
        //{
        //    await FillAdditionalDocuments(model, originalBasicDocumentId);
        //}

        public override async Task FillGrades(DiplomaCreateModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), nameof(DiplomaCreateModel));
            }

            if (!model.PersonId.HasValue || model.BasicDocumentTemplate == null || model.BasicDocumentTemplate.Parts.IsNullOrEmpty())
            {
                return;
            }

            var originalDiploma = _context.Diplomas.Where(i => i.PersonId == model.PersonId && i.BasicDocumentId == originalBasicDocumentId && i.IsPublic).FirstOrDefault();

            if (originalDiploma != null)
            {
                // Резултати от проф. подготовка
                var originalProfDocumentPart = _context.BasicDocumentParts.FirstOrDefault(i => i.BasicDocumentId == originalBasicDocumentId && i.Code == "1");
                var profDocumentPart = model.BasicDocumentTemplate.Parts.FirstOrDefault(i => i.Code == "1");
                var profSubjects = await GetSubjects(originalDiploma.Id, originalProfDocumentPart.Id);
                profDocumentPart.Subjects.AddRange(profSubjects);

                // ДИППК
                var originalDippkDocumentPart = _context.BasicDocumentParts.FirstOrDefault(i => i.BasicDocumentId == originalBasicDocumentId && i.Code == "2");
                var dippkDocumentPart = model.BasicDocumentTemplate.Parts.FirstOrDefault(i => i.Code == "2");
                var dippkSubjects = await GetSubjects(originalDiploma.Id, originalDippkDocumentPart.Id);
                dippkDocumentPart.Subjects.AddRange(dippkSubjects);

                model.Messages.Add(new DiplomaMessage() { Message = $"Добавени са изучавани предмети от оригинален документ {originalDiploma.RegistrationNumberTotal}-{originalDiploma.RegistrationNumberYear}/{(originalDiploma.RegistrationDate.HasValue ? originalDiploma.RegistrationDate.Value.ToString("dd.MM.yyyy"):"")}" });
            }
        }
    }
}
