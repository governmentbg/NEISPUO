namespace SB.Domain;

using FluentValidation;

public class CreateClassBookTopicPlanItemsFromExcelImportCommandValidator : AbstractValidator<CreateClassBookTopicPlanItemsFromExcelImportCommand>
{
    public CreateClassBookTopicPlanItemsFromExcelImportCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.CurriculumId).NotNull();
        this.RuleFor(c => c.BlobId).NotNull();
    }
}
