namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record UpdateLectureScheduleCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<LectureSchedule> LectureScheduleAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<UpdateLectureScheduleCommand, int>
{
    public async Task<int> Handle(UpdateLectureScheduleCommand command, CancellationToken ct)
    {
        if (!await this.ClassBookCachedQueryStore.CheckSchoolYearAllowsModificationsAsync(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            ct))
        {
            throw new DomainValidationException($"The school year is locked.");
        }

        var lectureSchedule = await this.LectureScheduleAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.LectureScheduleId!.Value,
            ct);

        if (command.InstId!.Value != lectureSchedule.InstId)
        {
            // the instId check is required as it is part of the auth checks
            throw new DomainValidationException($"Incorrect {nameof(command.InstId)}.");
        }

        // TeacherPersonId, StartDate and EndDate are passed in the command
        // only as a perf optimization as they are needed in the validator
        // and should not be different than the ones contained in the teacher absence
        if (lectureSchedule.TeacherPersonId != command.TeacherPersonId!.Value ||
            lectureSchedule.StartDate != command.StartDate!.Value ||
            lectureSchedule.EndDate != command.EndDate!.Value)
        {
            throw new DomainValidationException("Cannot change the TeacherPersonId, StaffPositionId, StartDate or EndDate!");
        }

        lectureSchedule.UpdateData(
            command.OrderNumber!,
            command.OrderDate!.Value,
            command.ScheduleLessonIds!,
            command.SysUserId!.Value);

        await this.UnitOfWork.SaveAsync(ct);

        return lectureSchedule.LectureScheduleId;
    }
}
