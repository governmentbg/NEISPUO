namespace SB.Domain;

using System.Linq;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SB.Common;

public class RemoveTeacherAbsenceCommandValidator : AbstractValidator<RemoveTeacherAbsenceCommand>
{
    public RemoveTeacherAbsenceCommandValidator()
    {
        this.ClassLevelCascadeMode = CascadeMode.Stop;

        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.TeacherAbsenceId).NotNull();

        this.RuleFor(c => c)
            .CustomAsync(async (c, context, ct) =>
            {
                var teacherAbsencesQueryRepository = context.GetServiceProvider().GetRequiredService<ITeacherAbsencesQueryRepository>();
                var teacherAbsenceHoursInUse = await teacherAbsencesQueryRepository.GetTeacherAbsenceHoursInUseAsync(
                    c.SchoolYear!.Value,
                    c.InstId!.Value,
                    c.TeacherAbsenceId!.Value,
                    ct);

                if (teacherAbsenceHoursInUse.Any())
                {
                    var hoursString = string.Join(
                        ", ",
                        teacherAbsenceHoursInUse.Select(h => $"{h.ClassName}/{h.Date.ToString("dd.MM.yyyy")}/#{h.HourNumber}"));
                    context.AddUserFailure(
                        $"Учителското отсъствие не може да бъде изтрито, защото има часове, за които има въведени оценки/отсътвия/теми - {hoursString.TruncateWithEllipsis(100)}.");
                }

                if (await teacherAbsencesQueryRepository.HasInvalidClassBooksForTeacherAbsenceAsync(
                        c.SchoolYear!.Value,
                        c.InstId!.Value,
                        c.TeacherAbsenceId!.Value,
                        ct))
                {
                    context.AddUserFailure(
                        "Учителското отсъствие не може да бъде изтрито, защото има часове, които са от архивиран дневник. " +
                        "Ако желаете да премахнете отсъствията за останалите часове, моля маркирайте ги като \"Учителя присъства\" и оставете архивираните за преглед.");
                }

            });
    }
}
