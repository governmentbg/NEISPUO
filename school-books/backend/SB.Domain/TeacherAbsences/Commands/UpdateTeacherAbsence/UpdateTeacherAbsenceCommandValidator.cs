namespace SB.Domain;

using System.Linq;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SB.Common;

public class UpdateTeacherAbsenceCommandValidator : AbstractValidator<UpdateTeacherAbsenceCommand>
{
    public UpdateTeacherAbsenceCommandValidator(IValidator<CreateTeacherAbsenceCommand> createValidator)
    {
        this.RuleFor(c => (CreateTeacherAbsenceCommand)c).SetValidator(createValidator);
        this.RuleFor(s => s.TeacherAbsenceId).NotNull();

        this.RuleFor(c => c)
            .CustomAsync(async (c, context, ct) =>
            {
                var teacherAbsencesQueryRepository = context.GetServiceProvider().GetRequiredService<ITeacherAbsencesQueryRepository>();

                var teacherAbsenceHoursInUse = await teacherAbsencesQueryRepository.GetTeacherAbsenceHoursInUseAsync(
                    c.SchoolYear!.Value,
                    c.InstId!.Value,
                    c.TeacherAbsenceId!.Value,
                    ct);

                var hoursSet = c.Hours!.Select(h => (
                    ScheduleLessonId: h.ScheduleLessonId!.Value,
                    h.ReplTeacherPersonId,
                    h.ReplTeacherIsNonSpecialist
                )).ToHashSet();

                var hoursInUseSet = teacherAbsenceHoursInUse.Select(h => (
                    h.ScheduleLessonId,
                    h.ReplTeacherPersonId,
                    h.ReplTeacherIsNonSpecialist
                )).ToHashSet();

                if (!hoursSet.IsSupersetOf(hoursInUseSet))
                {
                    hoursInUseSet.ExceptWith(hoursSet);

                    var hoursString = string.Join(
                        ", ",
                        teacherAbsenceHoursInUse
                        .Where(h => hoursInUseSet.Contains((
                            h.ScheduleLessonId,
                            h.ReplTeacherPersonId,
                            h.ReplTeacherIsNonSpecialist
                        )))
                        .Select(h => $"{h.ClassName}/{h.Date:dd.MM.yyyy}/#{h.HourNumber}"));
                    context.AddUserFailure(
                        $"При редакция на учителско отсъствие не могат да бъдат редактирани или да отпадат часове, за които има въведени оценки/отсътвия/теми - {hoursString.TruncateWithEllipsis(100)}.");
                }
            });
    }
}
