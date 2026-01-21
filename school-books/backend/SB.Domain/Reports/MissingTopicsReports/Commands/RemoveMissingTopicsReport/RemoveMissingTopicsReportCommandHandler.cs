namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveMissingTopicsReportCommandHandler(
    IMissingTopicsReportsAggregateRepository MissingTopicsReportsAggregateRepository)
    : IRequestHandler<RemoveMissingTopicsReportCommand>
{
    public async Task Handle(RemoveMissingTopicsReportCommand command, CancellationToken ct)
    {
        await this.MissingTopicsReportsAggregateRepository.RemoveAsync(
            command.SchoolYear!.Value,
            command.MissingTopicsReportId!.Value,
            ct);
    }
}
