namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveLectureSchedulesReportCommandHandler(
    ILectureSchedulesReportsAggregateRepository LectureSchedulesReportsAggregateRepository)
    : IRequestHandler<RemoveLectureSchedulesReportCommand>
{
    public async Task Handle(RemoveLectureSchedulesReportCommand command, CancellationToken ct)
    {
        await this.LectureSchedulesReportsAggregateRepository.RemoveAsync(
            command.SchoolYear!.Value,
            command.LectureSchedulesReportId!.Value,
            ct);
    }
}
