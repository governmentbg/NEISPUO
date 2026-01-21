namespace SB.Domain;

using FluentValidation;


public class UpdateGradeResultExtApiCommandValidator : AbstractValidator<UpdateGradeResultExtApiCommand>
{
    public UpdateGradeResultExtApiCommandValidator(IValidator<CreateUpdateGradeResultExtApiCommandCurriculum> retakeExamCurriculumValidator)
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.GradeResultId).NotNull();

        this.RuleFor(c => c.InitialResultType).NotEmpty().IsInEnum();
        this.RuleFor(c => c.RetakeExamCurriculums).NotNull();
        this.RuleForEach(c => c.RetakeExamCurriculums).SetValidator(retakeExamCurriculumValidator);
    }
}
