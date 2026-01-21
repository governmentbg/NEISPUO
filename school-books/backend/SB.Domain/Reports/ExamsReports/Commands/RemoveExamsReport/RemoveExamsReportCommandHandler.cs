namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record RemoveExamsReportCommandHandler(
        IExamsReportsAggregateRepository ExamsReportAggregateRepository)
    : IRequestHandler<RemoveExamsReportCommand>
{
    public async Task Handle(RemoveExamsReportCommand command, CancellationToken ct)
    {
        await this.ExamsReportAggregateRepository.RemoveAsync(
            command.SchoolYear!.Value,
            command.ExamsReportId!.Value,
            ct);
    }
}
