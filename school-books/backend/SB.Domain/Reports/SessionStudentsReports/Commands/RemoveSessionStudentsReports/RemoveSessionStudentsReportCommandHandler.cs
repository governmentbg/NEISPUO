namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record RemoveSessionStudentsReportCommandHandler(
        ISessionStudentsReportsAggregateRepository SessionStudentsReportAggregateRepository)
    : IRequestHandler<RemoveSessionStudentsReportCommand>
{
    public async Task Handle(RemoveSessionStudentsReportCommand command, CancellationToken ct)
    {
        await this.SessionStudentsReportAggregateRepository.RemoveAsync(
            command.SchoolYear!.Value,
            command.SessionStudentsReportId!.Value,
            ct);
    }
}
