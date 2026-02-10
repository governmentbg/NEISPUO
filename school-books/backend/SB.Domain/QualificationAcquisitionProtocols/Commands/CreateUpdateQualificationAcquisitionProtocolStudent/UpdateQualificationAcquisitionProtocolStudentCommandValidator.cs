namespace SB.Domain;

using FluentValidation;


public class UpdateQualificationAcquisitionProtocolStudentCommandValidator : AbstractValidator<UpdateQualificationAcquisitionProtocolStudentCommand>
{
    public UpdateQualificationAcquisitionProtocolStudentCommandValidator(IValidator<CreateQualificationAcquisitionProtocolStudentCommand> createValidator)
    {
        this.RuleFor(s => (CreateQualificationAcquisitionProtocolStudentCommand)s).SetValidator(createValidator, "Common");
    }
}
