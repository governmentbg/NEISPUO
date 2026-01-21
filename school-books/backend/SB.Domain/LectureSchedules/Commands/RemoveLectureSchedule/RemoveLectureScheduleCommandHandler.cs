namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record RemoveLectureScheduleCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<LectureSchedule> LectureScheduleAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<RemoveLectureScheduleCommand>
{
    public async Task Handle(RemoveLectureScheduleCommand command, CancellationToken ct)
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

        this.LectureScheduleAggregateRepository.Remove(lectureSchedule);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
