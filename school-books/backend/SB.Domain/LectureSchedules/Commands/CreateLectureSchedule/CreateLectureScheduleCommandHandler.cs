namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateLectureScheduleCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<LectureSchedule> LectureScheduleAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<CreateLectureScheduleCommand, int>
{
    public async Task<int> Handle(CreateLectureScheduleCommand command, CancellationToken ct)
    {
        if (!await this.ClassBookCachedQueryStore.CheckSchoolYearAllowsModificationsAsync(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            ct))
        {
            throw new DomainValidationException($"The school year is locked.");
        }

        var teacherLecture = new LectureSchedule(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.TeacherPersonId!.Value,
            command.OrderNumber!,
            command.OrderDate!.Value,
            command.StartDate!.Value,
            command.EndDate!.Value,
            command.ScheduleLessonIds!,
            command.SysUserId!.Value);

        await this.LectureScheduleAggregateRepository.AddAsync(teacherLecture, ct);
        await this.UnitOfWork.SaveAsync(ct);

        return teacherLecture.LectureScheduleId;
    }
}
