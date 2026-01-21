namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record RemoveStudentsAtRiskOfDroppingOutReportCommandHandler(
        IStudentsAtRiskOfDroppingOutReportAggregateRepository StudentsAtRiskOfDroppingOutReportAggregateRepository)
    : IRequestHandler<RemoveStudentsAtRiskOfDroppingOutReportCommand>
{
    public async Task Handle(RemoveStudentsAtRiskOfDroppingOutReportCommand command, CancellationToken ct)
    {
        await this.StudentsAtRiskOfDroppingOutReportAggregateRepository.RemoveAsync(
            command.SchoolYear!.Value,
            command.StudentsAtRiskOfDroppingOutReportId!.Value,
            ct);
    }
}
