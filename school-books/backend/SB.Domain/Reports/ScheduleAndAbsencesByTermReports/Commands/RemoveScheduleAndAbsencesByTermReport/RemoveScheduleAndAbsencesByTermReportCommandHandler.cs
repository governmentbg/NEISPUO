namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveScheduleAndAbsencesByTermReportCommandHandler(
    IScheduleAndAbsencesByTermReportsAggregateRepository ScheduleAndAbsencesByTermReportsAggregateRepository)
    : IRequestHandler<RemoveScheduleAndAbsencesByTermReportCommand>
{
    public async Task Handle(RemoveScheduleAndAbsencesByTermReportCommand command, CancellationToken ct)
    {
        await this.ScheduleAndAbsencesByTermReportsAggregateRepository.RemoveAsync(
            command.SchoolYear!.Value,
            command.ScheduleAndAbsencesByTermReportId!.Value,
            ct);
    }
}
