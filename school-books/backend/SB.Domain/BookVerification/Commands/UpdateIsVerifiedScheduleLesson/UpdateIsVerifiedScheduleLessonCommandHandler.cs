namespace SB.Domain;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record UpdateIsVerifiedScheduleLessonCommandHandler(
    IUnitOfWork UnitOfWork,
    ISchedulesAggregateRepository SchedulesAggregateRepository)
    : IRequestHandler<UpdateIsVerifiedScheduleLessonCommand>
{
    public async Task Handle(UpdateIsVerifiedScheduleLessonCommand command, CancellationToken ct)
    {
        var scheduleLessons = await this.SchedulesAggregateRepository.FindScheduleLessonsAsync(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.ScheduleLessons!.Select(sl => sl.ScheduleLessonId!.Value).ToArray(),
            ct);

        var scheduleLessonIsVerified = command.ScheduleLessons!.ToDictionary(
            sl => sl.ScheduleLessonId!.Value,
            sl => sl.IsVerified!.Value);

        foreach(var scheduleLesson in scheduleLessons)
        {
            scheduleLesson.UpdateIsVerified(
                scheduleLessonIsVerified[scheduleLesson.ScheduleLessonId],
                command.SysUserId!.Value);
        }

        await this.UnitOfWork.SaveAsync(ct);
    }
}
