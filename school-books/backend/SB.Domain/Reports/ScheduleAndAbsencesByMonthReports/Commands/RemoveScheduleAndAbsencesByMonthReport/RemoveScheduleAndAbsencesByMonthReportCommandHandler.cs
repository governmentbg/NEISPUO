namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveScheduleAndAbsencesByMonthReportCommandHandler(
    IScheduleAndAbsencesByMonthReportsAggregateRepository ScheduleAndAbsencesByMonthReportsAggregateRepository)
    : IRequestHandler<RemoveScheduleAndAbsencesByMonthReportCommand>
{
    public async Task Handle(RemoveScheduleAndAbsencesByMonthReportCommand command, CancellationToken ct)
    {
        await this.ScheduleAndAbsencesByMonthReportsAggregateRepository.RemoveAsync(
            command.SchoolYear!.Value,
            command.ScheduleAndAbsencesByMonthReportId!.Value,
            ct);
    }
}
