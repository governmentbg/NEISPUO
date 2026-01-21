namespace SB.Domain;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateTeacherAbsenceCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<TeacherAbsence> TeacherAbsenceAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<CreateTeacherAbsenceCommand, int>
{
    public async Task<int> Handle(CreateTeacherAbsenceCommand command, CancellationToken ct)
    {
        if (!await this.ClassBookCachedQueryStore.CheckSchoolYearAllowsModificationsAsync(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            ct))
        {
            throw new DomainValidationException($"The school year is locked.");
        }

        var teacherAbsence = new TeacherAbsence(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.TeacherPersonId!.Value, // TODO check teacher belongs to institution
            command.StartDate!.Value,
            command.EndDate!.Value,
            command.Reason!,
            command.Hours!.Select(h => (
                scheduleLessonId: h.ScheduleLessonId!.Value, // TODO check schedule lesson belongs to institution
                replTeacherPersonId: h.ReplTeacherPersonId, // TODO check teacher belongs to institution
                replTeacherIsNonSpecialist: h.ReplTeacherIsNonSpecialist,
                extReplTeacherName: h.ExtReplTeacherName
            )).ToArray(),
            command.SysUserId!.Value);

        await this.TeacherAbsenceAggregateRepository.AddAsync(teacherAbsence, ct);
        await this.UnitOfWork.SaveAsync(ct);

        return teacherAbsence.TeacherAbsenceId;
    }
}
