namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record RemoveAbsencesByClassesReportCommandHandler(
        IAbsencesByClassesReportsAggregateRepository AbsencesByClassesReportsAggregateRepository)
    : IRequestHandler<RemoveAbsencesByClassesReportCommand>
{
    public async Task Handle(RemoveAbsencesByClassesReportCommand command, CancellationToken ct)
    {
        await this.AbsencesByClassesReportsAggregateRepository.RemoveAsync(
            command.SchoolYear!.Value,
            command.AbsencesByClassesReportId!.Value,
            ct);
    }
}
