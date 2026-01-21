namespace SB.Domain;

using FluentValidation;

public class UpdateSanctionCommandValidator : AbstractValidator<UpdateSanctionCommand>
{
    public UpdateSanctionCommandValidator()
    {
        this.RuleFor(c => c.SanctionId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();

        this.RuleFor(c => c.SanctionTypeId).NotNull();
        this.RuleFor(c => c.OrderNumber).NotEmpty().MaximumLength(100);
        this.RuleFor(c => c.OrderDate).NotEmpty();
        this.RuleFor(c => c.StartDate).NotEmpty().GreaterThanOrEqualTo(c => c.OrderDate)
            .WithUserMessage("Начална дата не трябва да е преди Дата на заповед."); ;
        this.RuleFor(c => c.EndDate).GreaterThanOrEqualTo(c => c.StartDate)
            .WithUserMessage("Начална дата трябва да е преди Крайна дата.");
        this.RuleFor(c => c.Description).MaximumLength(1000);
        this.RuleFor(c => c.CancelOrderNumber).MaximumLength(100);
        this.RuleFor(c => c.CancelReason).MaximumLength(1000);
        this.RuleFor(c => c.RuoOrderNumber).MaximumLength(100);
    }
}
