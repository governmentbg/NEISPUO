namespace SB.Domain;

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

public class RemoveShiftCommandValidator : AbstractValidator<RemoveShiftCommand>
{
    public RemoveShiftCommandValidator()
    {
        this.ClassLevelCascadeMode = CascadeMode.Stop;

        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.ShiftId).NotNull();

        this.RuleFor(c => c)
            .CustomAsync(async (c, context, ct) =>
            {
                var shiftsQueryRepository = context.GetServiceProvider().GetRequiredService<IShiftsQueryRepository>();

                if (await shiftsQueryRepository.HasLinkedEntitiesAsync(
                    c.SchoolYear!.Value,
                    c.ShiftId!.Value,
                    ct))
                {
                    context.AddUserFailure("Не може да изтриете смяната, защото се използва.");
                }
            });
    }
}
