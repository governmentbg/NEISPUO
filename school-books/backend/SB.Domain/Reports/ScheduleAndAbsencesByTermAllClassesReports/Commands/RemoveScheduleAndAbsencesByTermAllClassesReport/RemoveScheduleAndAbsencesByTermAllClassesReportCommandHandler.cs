namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveScheduleAndAbsencesByTermAllClassesReportCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<ScheduleAndAbsencesByTermAllClassesReport> ScheduleAndAbsencesByTermAllClassesReportsAggregateRepository)
    : IRequestHandler<RemoveScheduleAndAbsencesByTermAllClassesReportCommand>
{
    public async Task Handle(RemoveScheduleAndAbsencesByTermAllClassesReportCommand command, CancellationToken ct)
    {
        var report = await this.ScheduleAndAbsencesByTermAllClassesReportsAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.ScheduleAndAbsencesByTermAllClassesReportId!.Value,
            ct);

        this.ScheduleAndAbsencesByTermAllClassesReportsAggregateRepository.Remove(report);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
