namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveAbsencesByStudentsReportCommandHandler(
    IAbsencesByStudentsReportsAggregateRepository AbsencesByStudentsReportsAggregateRepository)
    : IRequestHandler<RemoveAbsencesByStudentsReportCommand>
{
    public async Task Handle(RemoveAbsencesByStudentsReportCommand command, CancellationToken ct)
    {
        await this.AbsencesByStudentsReportsAggregateRepository.RemoveAsync(
            command.SchoolYear!.Value,
            command.AbsencesByStudentsReportId!.Value,
            ct);
    }
}
