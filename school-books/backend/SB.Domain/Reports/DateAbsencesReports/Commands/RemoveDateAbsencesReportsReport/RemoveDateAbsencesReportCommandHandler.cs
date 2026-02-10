namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveDateAbsencesReportCommandHandler(
    IDateAbsencesReportsAggregateRepository DateAbsencesReportsAggregateRepository)
    : IRequestHandler<RemoveDateAbsencesReportCommand>
{
    public async Task Handle(RemoveDateAbsencesReportCommand command, CancellationToken ct)
    {
        await this.DateAbsencesReportsAggregateRepository.RemoveAsync(
            command.SchoolYear!.Value,
            command.DateAbsencesReportId!.Value,
            ct);
    }
}
